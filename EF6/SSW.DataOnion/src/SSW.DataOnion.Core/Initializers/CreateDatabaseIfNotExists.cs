using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

using Serilog;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core.Initializers
{
    public class CreateDatabaseIfNotExists : IDatabaseInitializer
    {
        private readonly ILogger logger = Log.ForContext<CreateDatabaseIfNotExists>();

        public IDataSeeder DataSeeder { get; }

        public CreateDatabaseIfNotExists()
        {
            this.logger.Debug("DatabaseInitializer created without data seeder");
        }

        public CreateDatabaseIfNotExists(IDataSeeder dataSeeder)
        {
            this.DataSeeder = dataSeeder;
            this.logger.Debug("DatabaseInitializer created with data seeder");
        }

        public void Initialize<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            try
            {
                this.logger.Debug("Ensuring that database exists.");
                Database.SetInitializer(new CreateDatabaseIfNotExists<TDbContext>());
                var isCreated = DatabaseHelper.CheckIfDatabaseExists(dbContext, this.logger);
                dbContext.Database.Initialize(true);
                this.logger.Debug(isCreated ? "Database was not found. Created new database." : "Database already exists.");

                if (!isCreated || this.DataSeeder == null)
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

                if (!isCreated || this.DataSeeder == null)
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
