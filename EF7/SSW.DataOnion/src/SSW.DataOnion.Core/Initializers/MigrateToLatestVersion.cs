using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.Entity;

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
                // check if database seeder exists. If it does, we need to check if database has already been created
                // as there is no need to run data seeder if database already exists.
                var runSeed = false;
                if (this.DataSeeder != null)
                {
                    runSeed = !this.CheckIfDatabaseExists(dbContext);
                }

                this.logger.Debug("Run database migrations.");
                dbContext.Database.Migrate();
                
                if (!runSeed)
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
                // check if database seeder exists. If it does, we need to check if database has already been created
                // as there is no need to run data seeder if database already exists.
                var runSeed = false;
                if (this.DataSeeder != null)
                {
                    runSeed = await this.CheckIfDatabaseExistsAsync(dbContext, cancellationToken);
                }

                this.logger.Debug("Run database migrations.");
                await dbContext.Database.MigrateAsync(cancellationToken);

                if (!runSeed)
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

        private bool CheckIfDatabaseExists<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            try
            {
                dbContext.Database.OpenConnection();
                return true;
            }
            catch (SqlException ex)
            {
                this.logger.Debug(ex, "Database doesn't exist or connection string is invalid.");
                return false;
            }
        }

        private async Task<bool> CheckIfDatabaseExistsAsync<TDbContext>(
            TDbContext dbContext,
            CancellationToken cancellationToken = default(CancellationToken)) where TDbContext : DbContext
        {
            try
            {
                await dbContext.Database.OpenConnectionAsync(cancellationToken);
                return true;
            }
            catch (SqlException ex)
            {
                this.logger.Debug(ex, "Database doesn't exist or connection string is invalid.");
                return false;
            }
        }
    }
}
