using System;
using Microsoft.Extensions.DependencyInjection;

using SSW.DataOnion.Core;
using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.DependencyResolution.Microsoft
{
    public static class DataOnionExtensions
    {
        public static IServiceCollection AddDataOnion(this IServiceCollection serviceCollection, params DbContextConfig[] configs)
        {
            serviceCollection.AddTransient<IDbContextScope, DbContextScope>();
            serviceCollection.AddTransient<IDbContextReadOnlyScope, DbContextReadOnlyScope>();
            serviceCollection.AddTransient<IRepositoryResolver, MicrosoftRepositoryResolver>();
            serviceCollection.AddScoped<IDbContextScopeFactory, DbContextScopeFactory>();
            serviceCollection.AddScoped<IRepositoryLocator, RepositoryLocator>();
            serviceCollection.AddTransient<IAmbientDbContextLocator, AmbientDbContextLocator>();
            serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            serviceCollection.AddTransient<IReadOnlyUnitOfWork, ReadOnlyUnitOfWork>();
            serviceCollection.AddTransient<IDbContextReadOnlyScope, DbContextReadOnlyScope>();
            serviceCollection.AddTransient<IDbContextFactory>(
                container => new DbContextFactory(configs));

            serviceCollection.AddTransient<Func<IReadOnlyUnitOfWork>>(container => () => container.GetService<IReadOnlyUnitOfWork>());
            serviceCollection.AddTransient<Func<IDbContextReadOnlyScope>>(container => () => container.GetService<IDbContextReadOnlyScope>());

            return serviceCollection;
        }
    }
}
