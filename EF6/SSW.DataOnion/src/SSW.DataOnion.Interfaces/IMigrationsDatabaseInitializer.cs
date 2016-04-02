using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading;
using System.Threading.Tasks;

namespace SSW.DataOnion.Interfaces
{
    public interface IMigrationsDatabaseInitializer : IDatabaseInitializer
    {
        void Initialize<TDbContext, TMigrationsConfiguration>(
            TDbContext dbContext) 
            where TDbContext : DbContext
            where TMigrationsConfiguration : DbMigrationsConfiguration<TDbContext>, new();

        Task InitializeAsync<TDbContext, TMigrationsConfiguration>(
            TDbContext dbContext,
            CancellationToken cancellationToken = default(CancellationToken))
            where TDbContext : DbContext
            where TMigrationsConfiguration : DbMigrationsConfiguration<TDbContext>, new();
    }
}
