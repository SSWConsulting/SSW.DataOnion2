

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SSW.DataOnion.Core;

namespace SSW.DataOnion.EF6
{
    /// <summary>
    /// The base repository 
    /// </summary>
    /// <typeparam name="T">The type of the T.</typeparam>
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        /// <param name="contextManager">
        /// The context manager.
        /// </param>
        public BaseRepository(IDbContextManager contextManager)
        {
            if (contextManager == null)
            {
                throw new ArgumentNullException("contextManager");
            }

            this.ContextManager = contextManager;
        }

        /// <summary>
        /// Gets the context manager.
        /// </summary>
        protected IDbContextManager ContextManager { get; private set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected DbContext Context
        {
            get { return this.ContextManager.Context; }
        }

        /// <summary>
        /// Gets the database set.
        /// </summary>
        /// <value>The database set.</value>
        protected DbSet<T> DbSet
        {
            get { return this.Context.Set<T>(); }
        }

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <returns>Result set.</returns>
        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter)
        {
            return this.Get().Where(filter);
        }

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression and
        /// loaded with specified dependent objects.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>Materialized result set.</returns>
        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var get = this.Get();
            foreach (var include in includes)
            {
                get.Include(include);
            }

            return get.Where(filter).ToList();
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>Result set</returns>
        public virtual IQueryable<T> Get()
        {
            return this.DbSet;
        }

        /// <summary>
        /// Gets the read only records only.
        /// </summary>
        /// <returns>List of read-only records</returns>
        public virtual IQueryable<T> GetReadOnly()
        {
            return this.DbSet.AsNoTracking();
        }

        /// <summary>
        /// Finds the entity by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Found entity</returns>
        public virtual T Find(object id)
        {
            return this.DbSet.Find(id);
        }

        public Task<T> FindAsync(object id)
        {
            return this.DbSet.FindAsync(id);
        }

        /// <summary>
        /// Adds the specified entity to database context.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public virtual void Add(T entity)
        {
            this.DbSet.Add(entity);
        }


        /// <summary>
        /// Perform bulk insert of entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public virtual IEnumerable<T> BulkInsert(IEnumerable<T> entities)
        {
            return DbSet.AddRange(entities);
        }


        /// <summary>
        /// Perform bulk insert of entities, running the provided func on each entity prior to saving
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <param name="action">action to run on each entity</param>
        public virtual IEnumerable<T> BulkInsert(IEnumerable<T> entities, Action<T> action)
        {
            foreach (T entity in entities)
            {
                action(entity);
            }
            return this.BulkInsert(entities);
        }


        /// <summary>
        /// Deletes the specified entity by id.
        /// </summary>
        /// <param name="id">The entity id.</param>
        public virtual void Delete(object id)
        {
            T entityToDelete = this.Find(id);
            this.Delete(entityToDelete);
        }

     
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        public virtual void Delete(T entityToDelete)
        {
            if (this.Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToDelete);
            }

            this.DbSet.Remove(entityToDelete);
        }

        public async Task BulkInsertAsync(IEnumerable<T> entities)
        {
            await Task.Factory.StartNew(() => this.BulkInsert(entities));
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        public virtual void Update(T entityToUpdate)
        {
            if (this.Context.Entry(entityToUpdate).State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToUpdate);
                this.Context.Entry(entityToUpdate).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Loads specified entity with related collection.
        /// </summary>
        /// <typeparam name="TElement">The type of the collection element.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The collection expression.</param>
        public virtual void LoadCollection<TElement>(T entity, Expression<Func<T, ICollection<TElement>>> expression) where TElement : class
        {
            this.Context.Entry(entity).Collection(expression).Load();
        }

        /// <summary>
        /// Loads specified entity with related object.
        /// </summary>
        /// <typeparam name="TProperty">The type of the related object.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The object expression.</param>
        public virtual void LoadReference<TProperty>(T entity, Expression<Func<T, TProperty>> expression) where TProperty : class
        {
            this.Context.Entry(entity).Reference(expression).Load();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var get = this.Get();
            foreach (var include in includes)
            {
                get.Include(include);
            }

            return await get.Where(filter).ToListAsync();
        }
    }
}
