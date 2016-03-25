using System;
using Autofac;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.DependencyResolution.Autofac
{
    public class AutofacRepositoryResolver : IRepositoryResolver
    {
        private readonly ILifetimeScope lifetimeScope;

        public AutofacRepositoryResolver(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public IRepository<TEntity> Resolve<TEntity>() where TEntity : class
        {
            var repository = this.lifetimeScope.Resolve<IRepository<TEntity>>();

            if (repository == null)
            {
                throw new ApplicationException($"Could not resolve repository for entity of type {typeof(TEntity)}");
            }

            return repository;
        }
    }
}
