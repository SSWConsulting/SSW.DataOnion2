using Microsoft.Extensions.DependencyInjection;

using SSW.DataOnion.Core;
using SSW.DataOnion.Core.Initializers;
using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.DependencyResolution.Microsoft
{
    public static class DataOnionExtensions
    {
        public static IServiceCollection AddDataOnion(this IServiceCollection serviceCollection, string connectionString, IDatabaseInitializer databaseInitializer = null)
        {
            serviceCollection.AddTransient<IDbContextScope, DbContextScope>();
            serviceCollection.AddTransient<IDbContextReadOnlyScope, DbContextReadOnlyScope>();
            serviceCollection.AddTransient<IRepositoryResolver, MicrosoftRepositoryResolver>();
            serviceCollection.AddScoped<IDbContextScopeFactory, DbContextScopeFactory>();
            serviceCollection.AddScoped<IRepositoryLocator, RepositoryLocator>();
            serviceCollection.AddTransient<IAmbientDbContextLocator, AmbientDbContextLocator>();
            serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddTransient<IReadOnlyUnitOfWork, ReadOnlyUnitOfWork>();

            serviceCollection.AddTransient<IDbContextReadOnlyScope, DbContextReadOnlyScope>();
            serviceCollection.AddTransient<IDbContextFactory>(
                container => new DbContextFactory(connectionString, container.GetService<IDatabaseInitializer>()));

            if (databaseInitializer == null)
            {
                serviceCollection.AddTransient<IDatabaseInitializer, MigrateToLatestVersion>();
            }
            else
            {
                serviceCollection.AddTransient<IDatabaseInitializer, MigrateToLatestVersion>();
                serviceCollection.AddTransient(container => databaseInitializer);
            }

            return serviceCollection;
        }
    }
}
