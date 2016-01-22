using System;
using System.Threading.Tasks;

namespace SSW.DataOnion.Core
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Discards the changes by disposing each db context.
        /// </summary>
        void DiscardChanges();

        /// <summary>
        /// Saves the changes for each registered context.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Saves the changes for each registered context.
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
    }
}
