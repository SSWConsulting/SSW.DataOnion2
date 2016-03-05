using System;

using Microsoft.Extensions.DependencyInjection;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    public class MicrosoftRepositoryResolver : IRepositoryResolver
    {
        private readonly IServiceProvider serviceProvider;

        public MicrosoftRepositoryResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IRepository<TEntity> Resolve<TEntity>() where TEntity : class
        {
            return this.serviceProvider.GetService<IRepository<TEntity>>();
        }
    }
}
