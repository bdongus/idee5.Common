using System;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data {
    /// <summary>
    /// Unit of work pattern interface.
    /// </summary>
    public interface IUnitOfWork : IDisposable {
        /// <summary>
        /// Save changes asynchronously
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}