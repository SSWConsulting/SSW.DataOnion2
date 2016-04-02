using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SSW.DataOnion.Interfaces
{
    public interface IDataSeeder
    {
        void Seed<TDbContext>(TDbContext dbContext) where TDbContext : DbContext;

        Task SeedAsync<TDbContext>(
            TDbContext dbContext,
            CancellationToken cancellationToken = default(CancellationToken)) where TDbContext : DbContext;
    }
}
