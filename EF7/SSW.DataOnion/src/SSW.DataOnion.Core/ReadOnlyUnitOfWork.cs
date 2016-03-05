using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    public class ReadOnlyUnitOfWork : IReadOnlyUnitOfWork
    {
        private readonly IDbContextReadOnlyScope dbContextScope;

        private readonly IRepositoryLocator repositoryLocator;

        public ReadOnlyUnitOfWork(IDbContextReadOnlyScope dbContextScope, IRepositoryLocator repositoryLocator)
        {
            this.dbContextScope = dbContextScope;
            this.repositoryLocator = repositoryLocator;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return this.repositoryLocator.GetRepository<TEntity>();
        }

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.dbContextScope.Dispose();
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
