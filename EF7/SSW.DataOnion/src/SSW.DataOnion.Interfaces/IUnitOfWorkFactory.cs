using System;
namespace SSW.DataOnion.Interfaces
{
    /// <summary>
    /// Convenience methods to create a new ambient UnitOfWork. This is the prefered method
    /// to create a UnitOfWork.
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// Creates a new UnitOfWork.
        /// 
        /// By default, the new scope will join the existing ambient scope. This
        /// is what you want in most cases. This ensures that the same DbContext instances
        /// are used by all services methods called within the scope of a business transaction.
        /// </summary>
        IUnitOfWork Create();

        /// <summary>
        /// Creates a new UnitOfWork for read-only queries.
        /// 
        /// By default, the new scope will join the existing ambient scope. This
        /// is what you want in most cases. This ensures that the same DbContext instances
        /// are used by all services methods called within the scope of a business transaction.
        /// </summary>
        IReadOnlyUnitOfWork CreateReadOnly();

        /// <summary>
        /// Temporarily suppresses the ambient UnitOfWork. 
        /// 
        /// Always use this if you need to  kick off parallel tasks within a UnitOfWork. 
        /// This will prevent the parallel tasks from using the current ambient scope. If you
        /// were to kick off parallel tasks within a DbContextScope without suppressing the ambient
        /// context first, all the parallel tasks would end up using the same ambient DbContextScope, which 
        /// would result in multiple threads accesssing the same DbContext instances at the same 
        /// time.
        /// </summary>
        IDisposable SuppressAmbientContext();
    }
}
