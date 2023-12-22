using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// Defines the behavior of a repository of items. Used for simple CRUD operations.
/// Use <see cref="ICommandHandlerAsync{TCommand}"/> or <see cref="IQueryHandler{TQuery, TResult}"/> for complex scenarios.
/// </summary>
/// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
/// <typeparam name="TPrimaryKey"> The <see cref="Type"/> of the <see cref="IEntity{TPrimaryKey}"/> primary key.</typeparam>
public interface IRepository<T, TPrimaryKey> : IQueryRepository<T>
    where T : class, IEntity<TPrimaryKey>
    where TPrimaryKey : notnull {
    /// <summary>
    /// Adds a new item to the repository.
    /// </summary>
    /// <param name="item">The new item to add.</param>
    /// <remarks>The real work is done in the <see cref="IUnitOfWork"/>, so this stays synchronous.</remarks>
    void Add(T item);

    /// <summary>
    /// Add multiple items to the repository.
    /// </summary>
    /// <param name="items"> items to be added.</param>
    /// <remarks>The real work is done in the <see cref="IUnitOfWork"/>, so this stays synchronous.</remarks>
    void Add(IEnumerable<T> items);

    /// <summary>
    /// Removes an item from the repository.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <remarks>The real work is done in the <see cref="IUnitOfWork"/>, so this stays synchronous.</remarks>
    void Remove(T item);

    /// <summary>
    /// Remove multiple items.
    /// </summary>
    /// <param name="items">items to be removed.</param>
    /// <remarks>The real work is done in the <see cref="IUnitOfWork"/>, so this stays synchronous.</remarks>
    void Remove(IEnumerable<T> items);

    /// <summary>
    /// Removes all items meeting the given condition.
    /// </summary>
    /// <param name="predicate">Condition(s) to be met for removal.</param>
    /// <param name="cancellationToken">Token for operation cancellation.</param>
    /// <remarks>In most cases this hits the database for querying the predicate.</remarks>
    Task RemoveAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an item in the reopsitory.
    /// </summary>
    /// <param name="item">The item to update.</param>
    /// <returns>The <typeparamref name="T">updated item</typeparamref>.
    /// Generated values, like auditing fields, might differ from the input item.</returns>
    /// <remarks>The real work is done in the <see cref="IUnitOfWork"/>, so this stays synchronous.</remarks>
    void Update(T item);

    /// <summary>
    /// Performs a bulk <see cref="Action"/> on all items meeting the specified conditions.
    /// </summary>
    /// <param name="predicate">Condition(s) to be met.</param>
    /// <param name="action">Action to perform.</param>
    /// <param name="cancellationToken">Token for operation cancellation.</param>
    /// <remarks>In most cases this hits the database for querying the predicate.</remarks>
    Task ExecuteAsync(Expression<Func<T, bool>> predicate, Action<T> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update multiple items
    /// </summary>
    /// <param name="items">Updated items.</param>
    void Update(IEnumerable<T> items);

    /// <summary>
    /// Update an item in the repository. If it doesn't exist, it will be added.
    /// </summary>
    /// <param name="item">The item to update or add.</param>
    /// <param name="cancellationToken">Token for operation cancellation.</param>
    /// <remarks>In most cases this hits the database for querying the existence.</remarks>
    Task UpdateOrAddAsync(T item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update multiple items. If an item doesn't exist, it will be added.
    /// </summary>
    /// <param name="items">Items to update/add.</param>
    /// <param name="cancellationToken">Token for operation cancellation.</param>
    /// <remarks>In most cases this hits the database for querying the existence.</remarks>
    Task UpdateOrAddAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
}