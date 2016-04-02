using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    /// <summary>
    /// Factory for creating new instance of DbContext. Uses initializer and connection string to create new
    /// database instance
    /// </summary>
    public class DbContextFactory : IDbContextFactory
    {
        private readonly DbContextConfig[] dbContextConfigs;

        private static bool hasSetInitializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextFactory"/> class.
        /// </summary>
        /// <param name="dbContextConfigs">The database context configurations.</param>
        public DbContextFactory(params DbContextConfig[] dbContextConfigs)
        {
            Guard.AgainstNull(dbContextConfigs, nameof(dbContextConfigs));
            Guard.Against(
                () => this.InvalidConfigurations(dbContextConfigs),
                "At least one db context configuration must be specified for DbContextFactory");

            this.dbContextConfigs = dbContextConfigs;
        }

        /// <summary>
        /// Creates new instance of DbContext using specified initializer.
        /// </summary>
        /// <returns></returns>
        public virtual TDbContext Create<TDbContext>() where TDbContext : DbContext
        {
            var config = this.dbContextConfigs.FirstOrDefault(c => c.DbContextType == typeof (TDbContext));
            if (config == null)
            {
                throw new DataOnionException($"Could not find any configurations for DbContext of type {typeof(TDbContext)}");
            }
            var dbContext = (TDbContext)Activator.CreateInstance(typeof(TDbContext), config.ConnectionString);
            
            if (!hasSetInitializer)
            {
                config.DatabaseInitializer.Initialize(dbContext);

                hasSetInitializer = true;
            }

            return dbContext;
        }

        private IEnumerable<string> InvalidConfigurations(DbContextConfig[] dbContextConfigurations)
        {
            if (!dbContextConfigurations.Any())
            {
                yield return "At least one db context configuration must be specified for DbContextFactory";
            }
        } 
    }
}
