using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.Entity;

namespace SSW.DataOnion.Interfaces
{
    public interface IDatabaseInitializer
    {
        /// <summary>
        /// Gets the data seeder.
        /// </summary>
        /// <value> The data seeder. </value>
        IDataSeeder DataSeeder { get; }

        /// <summary>
        /// Initializes the specified database context. 
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <param name="dbContext">The database context.</param>
        void Initialize<TDbContext>(TDbContext dbContext) where TDbContext : DbContext;

        /// <summary>
        /// Initializes the specified database context. 
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <param name="dbContext">The database context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task InitializeAsync<TDbContext>(
            TDbContext dbContext,
            CancellationToken cancellationToken = default(CancellationToken)) where TDbContext : DbContext;
    }
}
