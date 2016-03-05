namespace SSW.DataOnion.Interfaces
{
    public interface IRepositoryResolver
    {
        IRepository<TEntity> Resolve<TEntity>() where TEntity : class;
    }
}
