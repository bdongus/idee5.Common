using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data {
    /// <summary>
    /// Abstract repository with basic implementations
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TKey">type of the entities primary key.</typeparam>
    public abstract class ARepository<T, TKey> : IRepository<T, TKey>
        where T : class, IEntity<TKey> {
        /// <inheritdoc />
        public abstract void Add(T item);

        /// <inheritdoc />
        public virtual void Add(IEnumerable<T> items) {
#pragma warning disable HAA0603 // Delegate allocation from a method group
            _ = items?.ForEach(Add);
#pragma warning restore HAA0603 // Delegate allocation from a method group
        }

        /// <inheritdoc />
        public abstract Task<IEnumerable<T>> GetAsync(Func<IQueryable<T>, IQueryable<T>> func, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<TResult> GetAsync<TResult>(Func<IQueryable<T>, TResult> func, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract void Remove(T item);

        /// <inheritdoc />
        public virtual void Remove(IEnumerable<T> items) {
#pragma warning disable HAA0603 // Delegate allocation from a method group
            _ = items?.ForEach(Remove);
#pragma warning restore HAA0603 // Delegate allocation from a method group
        }

        /// <inheritdoc />
        public abstract Task RemoveAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract void Update(T item);

        /// <inheritdoc />
        public virtual void Update(IEnumerable<T> items) {
#pragma warning disable HAA0603 // Delegate allocation from a method group
            items?.ForEach(Update);
#pragma warning restore HAA0603 // Delegate allocation from a method group
        }

        /// <inheritdoc />
        public abstract Task ExecuteAsync(Expression<Func<T, bool>> predicate, Action<T> action, CancellationToken cancellationToken = default);

#pragma warning disable HAA0302 // Display class allocation to capture closure
        /// <summary>
        /// Update an item in the repository. If it doesn't exist, it will be added.
        /// </summary>
        /// <param name="item">The item to update or add.</param>
        /// <param name="cancellationToken">Token for operation cancellation.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>In most cases this hits the database for querying the existence</item>
        /// <item>The default uses Count() to check for an items existence</item>
        /// </list>
        /// </remarks>
        public virtual async Task UpdateOrAddAsync(T item, CancellationToken cancellationToken = default) {
#pragma warning restore HAA0302 // Display class allocation to capture closure
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
#pragma warning disable HAA0301 // Closure Allocation Source
            bool exists = await GetAsync(q => q.Any(i => i.Id.Equals(item.Id))).ConfigureAwait(false);
#pragma warning restore HAA0301 // Closure Allocation Source
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
            if (exists)
                Add(item);
            else
                Update(item);
        }

        /// <inheritdoc />
        public virtual async Task UpdateOrAddAsync(IEnumerable<T> items, CancellationToken cancellationToken = default) {
            if (items?.Any() ?? false) {
                // Task.WhenAll is not possible with most ORM implementations, so do da for loop
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator
                foreach (T item in items) {
#pragma warning restore HAA0401 // Possible allocation of reference type enumerator
                    await UpdateOrAddAsync(item, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
