using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    public class RepositoryLocator : IRepositoryLocator
    {
        private readonly IRepositoryResolver resolver;

        public RepositoryLocator(IRepositoryResolver resolver)
        {
            this.resolver = resolver;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return this.resolver.Resolve<TEntity>();
        }
    }
}
