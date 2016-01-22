using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace SSW.DataOnion.EF6
{
    public interface IDbContextManager : IDisposable
    {
        bool HasContext { get; }

        DbContext Context { get; }
    }

    public interface IDbContextManager<T> : IDbContextManager
    {
    }

    public class DbContextManager<T> : IDbContextManager<T> 
        where T : DbContext
    {  
        IDbContextFactory<T> factory;

        public DbContextManager(IDbContextFactory<T> factory)
        {
            this.factory = factory;
        }

        T context = default(T);

        public DbContext Context
        {
            get
            {
                if (this.context == null)
                {  
                    this.context = this.factory.Create();
                }

                return this.context as DbContext;
            }
        }

        public bool HasContext
        {
            get
            {
                return this.context != null;
            }
        }

        public void Dispose()
        {
            if (this.HasContext)
            {
                this.context.Dispose();
                this.context = null;
            }
        }
    }
}
