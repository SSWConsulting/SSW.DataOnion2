using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Serilog;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core.Initializers
{
    public class MigrateToLatestVersion : IDatabaseInitializer
    {
        private readonly ILogger logger = Log.ForContext<MigrateToLatestVersion>();

        public IDataSeeder DataSeeder { get; }

        public MigrateToLatestVersion()
        {
            this.logger.Debug("DatabaseInitializer created without data seeder");
        }

        public MigrateToLatestVersion(IDataSeeder dataSeeder)
        {
            this.DataSeeder = dataSeeder;
            this.logger.Debug("DatabaseInitializer created with data seeder");
        }

        public void Initialize<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            try
            {
                this.logger.Debug("Checking if database exists.");
                var isCreated = DatabaseHelper.CheckIfDatabaseExists(dbContext, this.logger);

                this.logger.Debug("Attempt to locate migration configurations. If none found, create it.");
                var dbConfigurationType = typeof (TDbContext).Assembly.GetTypes().FirstOrDefault(t => t.BaseType == typeof(DbMigrationsConfiguration<TDbContext>));
                var configurations = dbConfigurationType == null
                    ? new DbMigrationsConfiguration<TDbContext> { AutomaticMigrationsEnabled = false }
                    : (DbMigrationsConfiguration<TDbContext>) Activator.CreateInstance(dbConfigurationType);

                this.logger.Debug("Run migrations.");
                var migrator = new DbMigrator(configurations);
                migrator.Update();

                if (isCreated || this.DataSeeder == null)
                {
                    return;
                }

                this.logger.Debug("Seed data");
                this.DataSeeder.Seed(dbContext);
            }
            catch (SqlException ex)
            {
                this.logger.Error(ex, "Error thrown during initialization");
            }
        }

        public async Task InitializeAsync<TDbContext>(
            TDbContext dbContext,
            CancellationToken cancellationToken = default(CancellationToken)) where TDbContext : DbContext
        {
            try
            {
                this.logger.Debug("Ensuring that database exists.");
                Database.SetInitializer(new CreateDatabaseIfNotExists<TDbContext>());
                var isCreated = DatabaseHelper.CheckIfDatabaseExists(dbContext, this.logger);
                this.logger.Debug(isCreated ? "Database was not found. Created new database." : "Database already exists.");

                if (isCreated || this.DataSeeder == null)
                {
                    return;
                }

                this.logger.Debug("Seed data");
                await this.DataSeeder.SeedAsync(dbContext, cancellationToken);
            }
            catch (SqlException ex)
            {
                this.logger.Error(ex, "Error thrown during initialization");
            }
        }
    }
}
