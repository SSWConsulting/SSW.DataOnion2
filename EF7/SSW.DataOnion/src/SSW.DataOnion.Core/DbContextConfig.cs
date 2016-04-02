using System;
using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    public class DbContextConfig
    {
        public DbContextConfig(string connectionString, Type dbContextType, IDatabaseInitializer databaseInitializer)
        {
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));
            Guard.AgainstNull(dbContextType, nameof(dbContextType));
            Guard.AgainstNull(databaseInitializer, nameof(databaseInitializer));

            this.ConnectionString = connectionString;
            this.DbContextType = dbContextType;
            this.DatabaseInitializer = databaseInitializer;
        }

        public string ConnectionString { get; }

        public Type DbContextType { get; }

        public IDatabaseInitializer DatabaseInitializer { get; }
    }
}
