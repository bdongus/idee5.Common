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
    /// Get a list of entities matching the query asynchronously.
    /// </summary>
    /// <param name="func">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IEnumerable{T}">sequence</see>
    /// of <typeparamref name="T">items</typeparamref>.</returns>
    Task<IEnumerable<T>> GetAsync(Func<IQueryable<T>, IQueryable<T>> func, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a query result asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The <see cref="Type">type</see> of result to retrieve.</typeparam>
    /// <param name="func">The <see cref="Func{T,TResult}"> function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task{T}">task</see> containing the <typeparamref name="TResult">result</typeparamref> of the operation.</returns>
    Task<TResult?> GetAsync<TResult>(Func<IQueryable<T>, TResult> func, CancellationToken cancellationToken = default);
}