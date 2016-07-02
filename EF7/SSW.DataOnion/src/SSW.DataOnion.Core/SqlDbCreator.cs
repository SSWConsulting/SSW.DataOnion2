using System.Data.SqlClient;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.Internal;

using Serilog;

namespace SSW.DataOnion.Core
{
    public class SqlDbCreator : SqlServerDatabaseCreator
    {
        private readonly ISqlServerConnection _connection;

        private static ILogger _logger = Log.ForContext<SqlDbCreator>();

        public SqlDbCreator(ISqlServerConnection connection, IMigrationsModelDiffer modelDiffer, IMigrationsSqlGenerator migrationsSqlGenerator, IMigrationCommandExecutor migrationCommandExecutor, IModel model, IRawSqlCommandBuilder rawSqlCommandBuilder) : base(connection, modelDiffer, migrationsSqlGenerator, migrationCommandExecutor, model, rawSqlCommandBuilder)
        {
            this._connection = connection;
        }

        public override bool Exists()
            => this.Exists(retryOnNotExists: false);

        private bool Exists(bool retryOnNotExists)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    this._connection.Open();
                    this._connection.Close();
                    return true;
                }
                catch (SqlException e)
                {
                    if (!retryOnNotExists
                        && IsDoesNotExist(e))
                    {
                        return false;
                    }

                    if (!this.RetryOnExistsFailure(e, ref retryCount))
                    {
                        return false;
                    }
                }
            }
        }


        // Login failed is thrown when database does not exist (See Issue #776)
        private static bool IsDoesNotExist(SqlException exception) => exception.Number == 4060;

        // See Issue #985
        private bool RetryOnExistsFailure(SqlException exception, ref int retryCount)
        {
            // This is to handle the case where Open throws (Number 233):
            //   System.Data.SqlClient.SqlException: A connection was successfully established with the
            //   server, but then an error occurred during the login process. (provider: Named Pipes
            //   Provider, error: 0 - No process is on the other end of the pipe.)
            // It appears that this happens when the database has just been created but has not yet finished
            // opening or is auto-closing when using the AUTO_CLOSE option. The workaround is to flush the pool
            // for the connection and then retry the Open call.
            // Also handling (Number -2):
            //   System.Data.SqlClient.SqlException: Connection Timeout Expired.  The timeout period elapsed while
            //   attempting to consume the pre-login handshake acknowledgement.  This could be because the pre-login
            //   handshake failed or the server was unable to respond back in time.
            // And (Number 4060):
            //   System.Data.SqlClient.SqlException: Cannot open database "X" requested by the login. The
            //   login failed.
            if (((exception.Number == 233) || (exception.Number == -2) || (exception.Number == 4060))
                && (++retryCount < 2))
            {
                this.ClearPool();
                Thread.Sleep(100);
                return true;
            }
            return false;
        }

        // Clear connection pool for the database connection since after the 'create database' call, a previously
        // invalid connection may now be valid.
        private void ClearPool() => SqlConnection.ClearPool((SqlConnection)this._connection.DbConnection);
    }
}
