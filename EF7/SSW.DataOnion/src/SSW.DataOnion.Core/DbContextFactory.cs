using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Serilog;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    /// <summary>
    /// Factory for creating new instance of DbContext. Uses initializer and connection string to create new
    /// database instance
    /// </summary>
    public class DbContextFactory : IDbContextFactory
    {
        private readonly ILogger logger = Log.ForContext<DbContextFactory>();

        private readonly string connectionString;

        private readonly IDatabaseInitializer databaseInitializer;

        private static bool hasSetInitializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseInitializer">Database initializer</param>
        public DbContextFactory(string connectionString, IDatabaseInitializer databaseInitializer)
        {
            this.connectionString = connectionString;
            this.databaseInitializer = databaseInitializer;
        }

        /// <summary>
        /// Creates new instance of DbContext using specified initializer.
        /// </summary>
        /// <returns></returns>
        public virtual TDbContext Create<TDbContext>() where TDbContext : DbContext
        {
            this.logger.Debug(
                "Creating new dbContext with connection string {connectionString}",this.connectionString);

            // create serviceProvider
            var serviceProvider = 
                new ServiceCollection()
                        .AddEntityFramework()
                        .AddSqlServer()
                        .AddDbContext<TDbContext>(
                            options =>
                                {
                                    options.UseSqlServer(this.connectionString);
                                })
                        .GetInfrastructure()
                        .Replace(
                            new ServiceDescriptor(
                                typeof(SqlServerDatabaseCreator), 
                                typeof(SqlDbCreator), 
                                ServiceLifetime.Scoped))
                        .BuildServiceProvider();


            var dbContext = serviceProvider.GetService<TDbContext>();

            if (hasSetInitializer)
            {
                return dbContext;
            }

            this.databaseInitializer.Initialize(dbContext);
            hasSetInitializer = true;

            return dbContext;
        }
    }
}
