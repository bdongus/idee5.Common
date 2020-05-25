using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data {
    public abstract class ACompositeKeyRepository<T> : ICompositeKeyRepository<T> where T : class {
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
            items?.ForEach(Remove);
#pragma warning restore HAA0603 // Delegate allocation from a method group
        }

        /// <inheritdoc />
        public abstract Task RemoveAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract void Update(T item);

        /// <inheritdoc />
        public void Update(IEnumerable<T> items) {
#pragma warning disable HAA0603 // Delegate allocation from a method group
            items?.ForEach(Update);
#pragma warning restore HAA0603 // Delegate allocation from a method group
        }

        /// <inheritdoc />
        public abstract Task ExecuteAsync(Expression<Func<T, bool>> predicate, Action<T> action, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task UpdateOrAddAsync(T item, CancellationToken cancellationToken = default);

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
