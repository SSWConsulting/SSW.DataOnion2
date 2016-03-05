using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    /// <summary>
    /// As its name suggests, DbContextCollection maintains a collection of DbContext instances.
    /// 
    /// What it does in a nutshell:
    /// - Lazily instantiates DbContext instances when its Get Of TDbContext () method is called
    /// (and optionally starts an explicit database transaction).
    /// - Keeps track of the DbContext instances it created so that it can return the existing
    /// instance when asked for a DbContext of a specific type.
    /// - Takes care of committing / rolling back changes and transactions on all the DbContext
    /// instances it created when its Commit() or Rollback() method is called.
    /// 
    /// </summary>
    public class DbContextCollection : IDbContextCollection
    {
        private readonly Dictionary<DbContext, IRelationalTransaction> transactions;
        private IsolationLevel? isolationLevel;
        private readonly IDbContextFactory dbContextFactory;
        private bool disposed;
        private bool completed;
        private readonly bool readOnly;

        internal Dictionary<Type, DbContext> InitializedDbContexts { get; }

        public DbContextCollection(bool readOnly = false, IsolationLevel? isolationLevel = null, IDbContextFactory dbContextFactory = null)
        {
            this.disposed = false;
            this.completed = false;

            this.InitializedDbContexts = new Dictionary<Type, DbContext>();
            this.transactions = new Dictionary<DbContext, IRelationalTransaction>();

            this.readOnly = readOnly;
            this.isolationLevel = isolationLevel;
            this.dbContextFactory = dbContextFactory;
        }

        public TDbContext Get<TDbContext>() where TDbContext : DbContext
        {
            if (this.disposed)
                throw new ObjectDisposedException("DbContextCollection");

            var requestedType = typeof(TDbContext);

            if (!this.InitializedDbContexts.ContainsKey(requestedType))
            {
                // First time we've been asked for this particular DbContext type.
                // Create one, cache it and start its database transaction if needed.
                var dbContext = this.dbContextFactory != null
                    ? this.dbContextFactory.Create<TDbContext>()
                    : Activator.CreateInstance<TDbContext>();

                this.InitializedDbContexts.Add(requestedType, dbContext);

                if (this.readOnly)
                {
                    dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                }

                if (this.isolationLevel.HasValue)
                {
                    var tran = dbContext.Database.BeginTransaction(this.isolationLevel.Value);
                    this.transactions.Add(dbContext, tran);
                }
            }

            return this.InitializedDbContexts[requestedType] as TDbContext;
        }

        public int Commit()
        {
            if (this.disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (this.completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            // Best effort. You'll note that we're not actually implementing an atomic commit 
            // here. It entirely possible that one DbContext instance will be committed successfully
            // and another will fail. Implementing an atomic commit would require us to wrap
            // all of this in a TransactionScope. The problem with TransactionScope is that 
            // the database transaction it creates may be automatically promoted to a 
            // distributed transaction if our DbContext instances happen to be using different 
            // databases. And that would require the DTC service (Distributed Transaction Coordinator)
            // to be enabled on all of our live and dev servers as well as on all of our dev workstations.
            // Otherwise the whole thing would blow up at runtime. 

            // In practice, if our services are implemented following a reasonably DDD approach,
            // a business transaction (i.e. a service method) should only modify entities in a single
            // DbContext. So we should never find ourselves in a situation where two DbContext instances
            // contain uncommitted changes here. We should therefore never be in a situation where the below
            // would result in a partial commit. 

            ExceptionDispatchInfo lastError = null;

            var c = 0;

            foreach (var dbContext in this.InitializedDbContexts.Values)
            {
                try
                {
                    if (!this.readOnly)
                    {
                        c += dbContext.SaveChanges();
                    }

                    // If we've started an explicit database transaction, time to commit it now.
                    var tran = GetValueOrDefault(this.transactions, dbContext);
                    if (tran != null)
                    {
                        tran.Commit();
                        tran.Dispose();
                    }
                }
                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }
            }

            this.transactions.Clear();
            this.completed = true;

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack track

            return c;
        }

        public Task<int> CommitAsync()
        {
            return this.CommitAsync(CancellationToken.None);
        }

        public async Task<int> CommitAsync(CancellationToken cancelToken)
        {
            if (cancelToken == null)
                throw new ArgumentNullException("cancelToken");
            if (this.disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (this.completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            // See comments in the sync version of this method for more details.

            ExceptionDispatchInfo lastError = null;

            var c = 0;

            foreach (var dbContext in this.InitializedDbContexts.Values)
            {
                try
                {
                    if (!this.readOnly)
                    {
                        c += await dbContext.SaveChangesAsync(cancelToken).ConfigureAwait(false);
                    }

                    // If we've started an explicit database transaction, time to commit it now.
                    var tran = GetValueOrDefault(this.transactions, dbContext);
                    if (tran != null)
                    {
                        tran.Commit();
                        tran.Dispose();
                    }
                }
                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }
            }

            this.transactions.Clear();
            this.completed = true;

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack track

            return c;
        }

        public void Rollback()
        {
            if (this.disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (this.completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            ExceptionDispatchInfo lastError = null;

            foreach (var dbContext in this.InitializedDbContexts.Values)
            {
                // There's no need to explicitly rollback changes in a DbContext as
                // DbContext doesn't save any changes until its SaveChanges() method is called.
                // So "rolling back" for a DbContext simply means not calling its SaveChanges()
                // method. 

                // But if we've started an explicit database transaction, then we must roll it back.
                var tran = GetValueOrDefault(this.transactions, dbContext);
                if (tran != null)
                {
                    try
                    {
                        tran.Rollback();
                        tran.Dispose();
                    }
                    catch (Exception e)
                    {
                        lastError = ExceptionDispatchInfo.Capture(e);
                    }
                }
            }

            this.transactions.Clear();
            this.completed = true;

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack track
        }

        public void Dispose()
        {
            if (this.disposed)
                return;

            // Do our best here to dispose as much as we can even if we get errors along the way.
            // Now is not the time to throw. Correctly implemented applications will have called
            // either Commit() or Rollback() first and would have got the error there.

            if (!this.completed)
            {
                try
                {
                    if (this.readOnly) this.Commit();
                    else this.Rollback();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

            foreach (var dbContext in this.InitializedDbContexts.Values)
            {
                try
                {
                    dbContext.Dispose();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

            this.InitializedDbContexts.Clear();
            this.disposed = true;
        }

        /// <summary>
        /// Returns the value associated with the specified key or the default 
        /// value for the TValue  type.
        /// </summary>
        private static TValue GetValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }
    }
}
