namespace SSW.DataOnion.Interfaces
{
    public interface IRepositoryLocator
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}
