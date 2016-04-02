using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.Core
{
    /// <summary>
    /// The base repository
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    public class BaseRepository<TEntity, TDbContext> : IRepository<TEntity>
        where TEntity : class where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the BaseRepository class.
        /// </summary>
        /// <param name="dbContextScopeLocator">
        /// The context manager.
        /// </param>
        public BaseRepository(IAmbientDbContextLocator dbContextScopeLocator)
        {
            if (dbContextScopeLocator == null)
            {
                throw new ArgumentNullException(nameof(dbContextScopeLocator));
            }

            this.DbContextScopeLocator = dbContextScopeLocator;
        }

        /// <summary>
        /// Gets the context manager.
        /// </summary>
        protected IAmbientDbContextLocator DbContextScopeLocator { get; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected TDbContext Context => this.DbContextScopeLocator.Get<TDbContext>();

        /// <summary>
        /// Gets the database set.
        /// </summary>
        /// <value>The database set.</value>
        protected DbSet<TEntity> DbSet => this.Context.Set<TEntity>();

        public IQueryable<TEntity> Get()
        {
            return this.DbSet;
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            return this.Get().Where(filter);
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes)
        {
            var get = this.Get();
            foreach (var include in includes)
            {
                get.Include(include);
            }

            return get.Where(filter);
        }

        public TEntity Find(params object[] keyValues)
        {
            return this.DbSet.Find(keyValues);
        }

        public void LoadCollection<TElement>(TEntity entity, Expression<Func<TEntity, ICollection<TElement>>> expression) where TElement : class
        {
            this.Context.Entry(entity).Collection(expression).Load();
        }

        public void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> expression) where TProperty : class
        {
            this.Context.Entry(entity).Reference(expression).Load();
        }

        public void Add(TEntity entity)
        {
            this.DbSet.Add(entity);
        }

        public void Activate(TEntity entityToActivate)
        {
            var baseEntity = entityToActivate as ISoftDeletableEntity;

            if (baseEntity == null)
            {
                return;
            }

            if (this.Context.Entry(entityToActivate).State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToActivate);
                this.Context.Entry(entityToActivate).State = EntityState.Modified;
            }

            baseEntity.IsDeleted = false;
        }

        public void Update(TEntity entityToUpdate)
        {
            if (this.Context.Entry(entityToUpdate).State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToUpdate);
                this.Context.Entry(entityToUpdate).State = EntityState.Modified;
            }
        }

        public void Delete(params object[] keyValues)
        {
            TEntity entityToDelete = this.Find(keyValues);
            this.Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            this.DbSet.Remove(entityToDelete);
        }

        public void DeleteRange(IEnumerable<TEntity> entityToDelete)
        {
            this.DbSet.RemoveRange(entityToDelete);
        }

        public void Deactivate(TEntity entityToDeactivate)
        {
            var baseEntity = entityToDeactivate as ISoftDeletableEntity;

            if (baseEntity == null)
            {
                return;
            }

            if (this.Context.Entry(entityToDeactivate).State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToDeactivate);
                this.Context.Entry(entityToDeactivate).State = EntityState.Modified;
            }

            baseEntity.IsDeleted = true;
        }
    }
}
