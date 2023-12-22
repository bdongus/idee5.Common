using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// Read only repository.
/// </summary>
/// <remarks>Useful for CQRS implementations.</remarks>
/// <typeparam name="T">Entity type.</typeparam>
public interface IQueryRepository<T> where T : class {
    /// <summary>
    /// Get a list of items matching the query asynchronously.
    /// </summary>
    /// <param name="func">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation</param>
    /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IEnumerable{T}">sequence</see> of items</returns>
    Task<IEnumerable<T>> GetAsync(Func<IQueryable<T>, IQueryable<T>> func, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if at least one item exists meeting the given criteria
    /// </summary>
    /// <param name="predicate">Search predicate</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation</param>
    /// <returns><c>True</c> if there is at least one item meeting the criteria</returns>
    Task<bool> ExistsAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count of items meeting the given criteria
    /// </summary>
    /// <param name="predicate">Search predicate</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation</param>
    /// <returns><c>True</c> if there is at least one item meeting the criteria</returns>
    Task<int> CountAsync(Func<T,bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single item in the repository matching the specified predicate asynchronously
    /// </summary>
    /// <param name="predicate">The predicate used to match the requested item</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation</param>
    /// <returns>A <see cref="Task{T}">task</see> containing the matched item or null if no match was found</returns>
    Task<T?> GetSingleAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default);
    }