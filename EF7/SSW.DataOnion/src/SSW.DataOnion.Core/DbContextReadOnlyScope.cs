using System.Data;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    public class DbContextReadOnlyScope : IDbContextReadOnlyScope
    {
        private DbContextScope _internalScope;

        public IDbContextCollection DbContexts { get { return this._internalScope.DbContexts; } }

        public DbContextReadOnlyScope(IDbContextFactory dbContextFactory = null)
            : this(joiningOption: DbContextScopeOption.JoinExisting, isolationLevel: null, dbContextFactory: dbContextFactory)
        { }

        public DbContextReadOnlyScope(IsolationLevel isolationLevel, IDbContextFactory dbContextFactory = null)
            : this(joiningOption: DbContextScopeOption.ForceCreateNew, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory)
        { }

        public DbContextReadOnlyScope(DbContextScopeOption joiningOption, IsolationLevel? isolationLevel, IDbContextFactory dbContextFactory = null)
        {
            this._internalScope = new DbContextScope(joiningOption: joiningOption, readOnly: true, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory);
        }

        public void Dispose()
        {
            this._internalScope.Dispose();
        }
    }
}
