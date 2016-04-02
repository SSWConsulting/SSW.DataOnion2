using System;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IDbContextScopeFactory dbContextScopeFactory;
        private readonly IRepositoryLocator repositoryLocator;

        public UnitOfWorkFactory(IDbContextScopeFactory dbContextScopeFactory, IRepositoryLocator repositoryLocator)
        {
            this.dbContextScopeFactory = dbContextScopeFactory;
            this.repositoryLocator = repositoryLocator;
        }

        public IUnitOfWork Create()
        {
            return new UnitOfWork(dbContextScopeFactory, repositoryLocator);
        }

        public IReadOnlyUnitOfWork CreateReadOnly()
        {
            return new ReadOnlyUnitOfWork(dbContextScopeFactory, repositoryLocator);
        }

        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSuppressor();
        }
    }
}
