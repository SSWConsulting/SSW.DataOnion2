using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace SSW.DataOnion.EF6
{
    public class DbContextFactory<T> : IDbContextFactory<T> where T : DbContext
    {
        private static bool hasSetInitializer;

        private readonly IDatabaseInitializer<T> dbInitializer;

        private readonly string connectionString;

        private readonly DbConnection connection = null;


        private readonly Action<T> configCallback = null;  


        public DbContextFactory(IDatabaseInitializer<T> dbInitializer, string connectionString)
        {
            this.dbInitializer = dbInitializer;
            this.connectionString = connectionString;
        }

        public DbContextFactory(IDatabaseInitializer<T> dbInitializer, DbConnection connection)
        {
            this.dbInitializer = dbInitializer;
            this.connection = connection;
        }


        public DbContextFactory(IDatabaseInitializer<T> dbInitializer, string connectionString, Action<T> configCallback)
        {
            this.dbInitializer = dbInitializer;
            this.connectionString = connectionString;
            this.configCallback = configCallback;
        }

        public DbContextFactory(IDatabaseInitializer<T> dbInitializer, DbConnection connection, Action<T> configCallback)
        {
            this.dbInitializer = dbInitializer;
            this.connection = connection;
            this.configCallback = configCallback;
        }


        public virtual T Create()
        {
            if (!hasSetInitializer)
            {
                Database.SetInitializer<T>(this.dbInitializer);
                hasSetInitializer = true;
            }

            Object[] args;
            if (this.connection != null) // are we using a dBconnection or a Connection string?
            {
                args = new Object[] { this.connection };
            }
            else
            {
                args = new Object[] { connectionString };
            }

            var ctx =  (T)Activator.CreateInstance(typeof(T), args);

            if (configCallback != null)
            {
                configCallback(ctx);
            }

            return ctx;
        }
    }
}
