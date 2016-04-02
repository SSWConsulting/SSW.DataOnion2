using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace SSW.DataOnion.Core.Initializers
{
    internal class DatabaseHelper
    {
        public static bool CheckIfDatabaseExists<TDbContext>(TDbContext dbContext, ILogger logger) where TDbContext : DbContext
        {
            try
            {
                // Check if database connnection can be made and if database has any non-system tables
                using (var connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT * FROM information_schema.tables where TABLE_SCHEMA != 'sys'", connection);
                    var reader = command.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (SqlException ex)
            {
                logger.Debug(ex, "Database doesn't exist or connection string is invalid.");
                return false;
            }
        }
    }
}
