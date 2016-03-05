using System;

namespace SSW.DataOnion.Interfaces
{
    public interface IReadOnlyUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    }
}
