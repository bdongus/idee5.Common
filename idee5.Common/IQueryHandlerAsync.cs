using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common {
    /// <summary>
    /// Query pattern: https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=92
    /// </summary>
    /// <typeparam name="TQuery">An <see cref="IQuery{TResult}"/></typeparam>
    /// <typeparam name="TResult">The result data type.</typeparam>
    public interface IQueryHandlerAsync<TQuery, TResult> where TQuery : IQuery<TResult> {
        /// <summary>
        /// Handle the query call asynchrnously.
        /// </summary>
        /// <param name="query">query parameter(s).</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Result <see cref="Task"/></returns>
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}