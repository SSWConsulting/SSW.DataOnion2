using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Serilog;
using SSW.DataOnion.Core;

namespace SSW.DataOnion.EF6
{
    public class UnitOfWork : IUnitOfWork
    {
        internal IEnumerable<IDbContextManager> contextList;

        private Serilog.ILogger _logger = Log.ForContext<UnitOfWork>();

        //internal bool disposed;

        public UnitOfWork(IEnumerable<IDbContextManager> contextList)
        {
            this.contextList = contextList;
        }

        public virtual void DiscardChanges()
        {
            try
            {
                foreach (var contextManager in this.contextList)
                {
                    if (contextManager.HasContext)
                    {
                        contextManager.Context.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        public virtual void SaveChanges()
        {
            try
            {
                foreach (var contextManager in this.contextList)
                {
                    if (contextManager.HasContext)
                    {
                        contextManager.Context.SaveChanges();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.Error(sqlEx.Message, sqlEx);
                var ex = sqlEx.ToDataOperationException();
                throw ex;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException due)
            {
                _logger.Error(due.Message, due);
                var ex = due.ToDataOperationException();
                throw ex;

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _logger.Error(string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage), dbEx);
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        public virtual async Task SaveChangesAsync()
        {
            try
            {
                foreach (var contextManager in this.contextList)
                {
                    if (contextManager.HasContext)
                    {
                        await contextManager.Context.SaveChangesAsync();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.Error(sqlEx.Message, sqlEx);
                var ex = sqlEx.ToDataOperationException();
                throw ex;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException due)
            {
                _logger.Error(due.Message, due);
                var ex = due.ToDataOperationException();
                throw ex;

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _logger.Error(string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage), dbEx);
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (!this.disposed)
            //{
                if (disposing)
                {
                    foreach (var contextManager in this.contextList)
                    {
                        if (contextManager != null)
                        {
                            contextManager.Dispose();
                        }
                    }
                }

                //this.disposed = true;
            //}
        }

        public void Dispose()
        {
            this.Dispose(true);
            //GC.SuppressFinalize(this);
        }
    }

    public static class SqlExceptionExtensions
    {

        public static DataOperationException ToDataOperationException(this System.Data.Entity.Infrastructure.DbUpdateException dbUpdateException)
        {
            DataOperationException result = null;

            Exception innerMostException = dbUpdateException.InnerMostException();
            var sqlException = innerMostException as SqlException; //if (innerMostException is SqlException) if (innerMostException.GetType().IsAssignableFrom(typeof(SqlException)))
            if (sqlException != null)
            {
                result = sqlException.ToDataOperationException();
            }
            else
            {
                result = new DataOperationException("Unknown Data Error", dbUpdateException);
            }
            return result;

        }

        public static DataOperationException ToDataOperationException(this SqlException ex)
        {
            DataOperationException dataException = null;
            if (ex.Errors.Count > 0) // Assume the interesting stuff is in the first error
            {
                switch (ex.Errors[0].Number)
                {
                    case 547: // Foreign Key violation
                        dataException = new DataOperationException("Foreign Key violation.", ex);
                        break;
                    case 2601: // Primary key violation
                        dataException = new DataOperationException("Primary key violation.", ex);
                        break;
                    default:
                        dataException = new DataOperationException("Unknown data exception.", ex);
                        break;
                }
            }
            return dataException;
        }


        public static Exception InnerMostException(this Exception ex)
        {
            var e = ex;
            while (e.InnerException != null) e = e.InnerException;
            return e;
        }

    }
}
