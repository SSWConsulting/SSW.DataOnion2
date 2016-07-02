using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Serilog;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core.Initializers
{
    public class DropCreateDatabaseAlways : IDatabaseInitializer
    {
        private readonly ILogger logger = Log.ForContext<DropCreateDatabaseAlways>();

        public IDataSeeder DataSeeder { get; }

        public DropCreateDatabaseAlways()
        {
            this.logger.Debug("DatabaseInitializer created without data seeder");
        }

        public DropCreateDatabaseAlways(IDataSeeder dataSeeder)
        {
            this.DataSeeder = dataSeeder;
            this.logger.Debug("DatabaseInitializer created with data seeder");
        }

        public void Initialize<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            try
            {
                this.logger.Debug("Ensuring that database is deleted.");
                var isDeleted = dbContext.Database.EnsureDeleted();
                this.logger.Debug(isDeleted ? "Database was found and deleted." : "Database was not found. Nothing to delete.");

                this.logger.Debug("Create database");
                var isCreated = dbContext.Database.EnsureCreated();

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
                this.logger.Debug("Ensuring that database is deleted.");
                var isDeleted = await dbContext.Database.EnsureDeletedAsync(cancellationToken);
                this.logger.Debug(isDeleted ? "Database was found and deleted." : "Database was not found. Nothing to delete.");

                this.logger.Debug("Create database");
                var isCreated = await dbContext.Database.EnsureCreatedAsync(cancellationToken);

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
