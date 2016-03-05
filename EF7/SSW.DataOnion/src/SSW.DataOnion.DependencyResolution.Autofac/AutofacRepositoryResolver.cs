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
            return this.lifetimeScope.Resolve<IRepository<TEntity>>();
        }
    }
}
