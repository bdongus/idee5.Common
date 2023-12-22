using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    /// Get a list of items matching the predicate asynchronously
    /// </summary>
    /// <param name="predicate">Search predicate to execute</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation</param>
    /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="List{T}">list</see> of items</returns>
    Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    /// <summary>
    /// Get all items asynchronously
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation</param>
    /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="List{T}">list</see> of items</returns>
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if at least one item exists meeting the given criteria
    /// </summary>
    /// <param name="predicate">Search predicate</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation</param>
    /// <returns><c>True</c> if there is at least one item meeting the criteria</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count of items meeting the given criteria
    /// </summary>
    /// <param name="predicate">Search predicate</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation</param>
    /// <returns><c>True</c> if there is at least one item meeting the criteria</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single item in the repository matching the specified predicate asynchronously
    /// </summary>
    /// <param name="predicate">The predicate used to match the requested item</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation</param>
    /// <returns>A <see cref="Task{T}">task</see> containing the matched item or null if no match was found</returns>
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }