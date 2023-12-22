using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// Abstract composite key repository.
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public abstract class ACompositeKeyRepository<T> : ICompositeKeyRepository<T> where T : class {
    /// <inheritdoc />
    public abstract void Add(T item);

    /// <inheritdoc />
    public virtual void Add(IEnumerable<T> items) {
        _ = items?.ForEach(Add);
    }

    /// <inheritdoc />
    public abstract void Remove(T item);

    /// <inheritdoc />
    public virtual void Remove(IEnumerable<T> items) {
        items?.ForEach(Remove);
    }

    /// <inheritdoc />
    public abstract Task RemoveAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract void Update(T item);

    /// <inheritdoc />
    public void Update(IEnumerable<T> items) {
        items?.ForEach(Update);
    }

    /// <inheritdoc />
    public abstract Task ExecuteAsync(Expression<Func<T, bool>> predicate, Action<T> action, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task UpdateOrAddAsync(T item, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public virtual async Task UpdateOrAddAsync(IEnumerable<T> items, CancellationToken cancellationToken = default) {
        if (items?.Any() ?? false) {
            // Task.WhenAll is not possible with most ORM implementations, so do da for loop
            foreach (T item in items) {
                await UpdateOrAddAsync(item, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    public abstract Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    /// <inheritdoc />
    public abstract Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    /// <inheritdoc />
    public abstract Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    /// <inheritdoc />
    public abstract Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    /// <inheritdoc />
    public abstract Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
}
