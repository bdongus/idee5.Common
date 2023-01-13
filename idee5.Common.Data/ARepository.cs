using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// Abstract repository with basic implementations
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TPrimaryKey">type of the entities primary key.</typeparam>
public abstract class ARepository<T, TPrimaryKey> : IRepository<T, TPrimaryKey>
    where T : class, IEntity<TPrimaryKey>
    where TPrimaryKey : notnull {
    /// <inheritdoc />
    public abstract void Add(T item);

    /// <inheritdoc />
    public virtual void Add(IEnumerable<T> items) {
        _ = items?.ForEach(Add);
    }

    /// <inheritdoc />
    public abstract Task<IEnumerable<T>> GetAsync(Func<IQueryable<T>, IQueryable<T>> func, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<TResult> GetAsync<TResult>(Func<IQueryable<T>, TResult> func, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract void Remove(T item);

    /// <inheritdoc />
    public virtual void Remove(IEnumerable<T> items) {
        _ = items?.ForEach(Remove);
    }

    /// <inheritdoc />
    public abstract Task RemoveAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract void Update(T item);

    /// <inheritdoc />
    public virtual void Update(IEnumerable<T> items) {
        _ = (items?.ForEach(Update));
    }

    /// <inheritdoc />
    public abstract Task ExecuteAsync(Expression<Func<T, bool>> predicate, Action<T> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an item in the repository. If it doesn't exist, it will be added.
    /// </summary>
    /// <param name="item">The item to update or add.</param>
    /// <param name="cancellationToken">Token for operation cancellation.</param>
    /// <remarks>
    /// <list type="bullet">
    /// <item>In most cases this hits the database for querying the existence</item>
    /// <item>The default uses Any() to check for an items existence</item>
    /// </list>
    /// </remarks>
    public virtual async Task UpdateOrAddAsync(T item, CancellationToken cancellationToken = default) {
        bool exists = await GetAsync(q => q.Any(i => i.Id.Equals(item.Id)), cancellationToken).ConfigureAwait(false);
        if (!exists)
            Add(item);
        else
            Update(item);
    }

    /// <inheritdoc />
    public virtual async Task UpdateOrAddAsync(IEnumerable<T> items, CancellationToken cancellationToken = default) {
        if (items?.Any() ?? false) {
            // Task.WhenAll is not possible with most ORM implementations, so do da for loop
            foreach (T item in items) {
                await UpdateOrAddAsync(item, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}