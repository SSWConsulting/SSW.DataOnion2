using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

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
                Database.SetInitializer(new DropCreateDatabaseAlways<TDbContext>());
                dbContext.Database.Initialize(true);

                if (this.DataSeeder == null)
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
                Database.SetInitializer(new DropCreateDatabaseAlways<TDbContext>());
                dbContext.Database.Initialize(true);

                if (this.DataSeeder == null)
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
