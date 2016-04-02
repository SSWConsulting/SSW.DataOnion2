using System;
using System.Data;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    public class DbContextScopeFactory : IDbContextScopeFactory
    {
        private readonly IDbContextFactory _dbContextFactory;

        public DbContextScopeFactory(IDbContextFactory dbContextFactory = null)
        {
            this._dbContextFactory = dbContextFactory;
        }

        public IDbContextScope Create(DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting)
        {
            return new DbContextScope(
                joiningOption: joiningOption,
                readOnly: false,
                isolationLevel: null,
                dbContextFactory: this._dbContextFactory);
        }

        public IDbContextReadOnlyScope CreateReadOnly(DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting)
        {
            return new DbContextReadOnlyScope(
                joiningOption: joiningOption,
                isolationLevel: null,
                dbContextFactory: this._dbContextFactory);
        }

        public IDbContextScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return new DbContextScope(
                joiningOption: DbContextScopeOption.ForceCreateNew,
                readOnly: false,
                isolationLevel: isolationLevel,
                dbContextFactory: this._dbContextFactory);
        }

        public IDbContextReadOnlyScope CreateReadOnlyWithTransaction(IsolationLevel isolationLevel)
        {
            return new DbContextReadOnlyScope(
                joiningOption: DbContextScopeOption.ForceCreateNew,
                isolationLevel: isolationLevel,
                dbContextFactory: this._dbContextFactory);
        }

        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSuppressor();
        }
    }
}
