using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data {
    /// <summary>
    /// Provides extension methods for the <see cref="IQueryRepository{T}"/> interface.
    /// Be careful! Using those can create a dependency to the used ORM.
    /// </summary>
    public static class RepositoryExtensions {
        /// <summary>
        /// Retrieves all items in the repository asynchronously.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
        /// <param name="queryRepository">The extended <see cref="IQueryRepository{T}">repository</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="IEnumerable{T}">sequence</see>
        /// of all <typeparamref name="T">items</typeparamref> in the repository.</returns>
        public static Task<IEnumerable<T>> GetAllAsync<T>(this IQueryRepository<T> queryRepository, CancellationToken cancellationToken = default) where T : class {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));
            return queryRepository.GetAsync<IEnumerable<T>>(q => q, cancellationToken);
        }

        /// <summary>
        /// Searches for items in the repository that match the specified predicate asynchronously.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
        /// <param name="queryRepository">The extended <see cref="IQueryRepository{T}">repository</see>.</param>
        /// <param name="expression">The <see cref="Expression{T}">expression</see> representing the predicate used to
        /// match the requested <typeparamref name="T">items</typeparamref>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the matched <see cref="IEnumerable{T}">sequence</see>
        /// of <typeparamref name="T">items</typeparamref>.</returns>
        public static Task<IEnumerable<T>> FindByAsync<T>(this IQueryRepository<T> queryRepository, Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) where T : class {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));
            return queryRepository.GetAsync<IEnumerable<T>>(q => q.Where(expression), cancellationToken);
        }

        /// <summary>
        /// Retrieves a single item in the repository matching the specified predicate asynchronously.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
        /// <param name="queryRepository">The extended <see cref="IQueryRepository{T}">repository</see>.</param>
        /// <param name="expression">The <see cref="Expression{T}">expression</see> representing the predicate used to
        /// match the requested <typeparamref name="T">item</typeparamref>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the matched <typeparamref name="T">item</typeparamref>
        /// or null if no match was found.</returns>
        public static Task<T> GetSingleAsync<T>(this IQueryRepository<T> queryRepository, Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) where T : class {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            return queryRepository.GetAsync(q => q.SingleOrDefault(expression), cancellationToken);
        }

        /// <summary>
        /// Retrieves and pages all items in the repository asynchronously.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
        /// <param name="queryRepository">The extended <see cref="IQueryRepository{T}">repository</see>.</param>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="pageSize">The size of the data page to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="PagedCollection{T}">paged collection</see>
        /// of <typeparamref name="T">items</typeparamref>.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required to support paging.")]
        public static async Task<PagedCollection<T>> PaginateAsync<T>(this IQueryRepository<T> queryRepository, int pageIndex, int pageSize, CancellationToken cancellationToken = default) where T : class {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            var groups = await queryRepository.GetAsync(
                q => {
                    var startIndex = pageIndex * pageSize;
                    return q.Skip(startIndex)
                            .Take(pageSize)
                            .GroupBy(
                                _ => new
                                {
                                    Total = q.Count()
                                });
                },
                cancellationToken).ConfigureAwait(false);

            // return first group
            var result = groups.FirstOrDefault();

            if (result == null)
                return new PagedCollection<T>(Enumerable.Empty<T>(), 0L);

            return new PagedCollection<T>(result, result.Key.Total);
        }

        /// <summary>
        /// Retrieves and pages matching items in the repository asynchronously.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
        /// <param name="queryRepository">The extended <see cref="IQueryRepository{T}">repository</see>.</param>
        /// <param name="queryShaper">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="pageSize">The size of the data page to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="PagedCollection{T}">paged collection</see>
        /// of <typeparamref name="T">items</typeparamref>.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required to support paging.")]
        public static async Task<PagedCollection<T>> PaginateAsync<T>(this IQueryRepository<T> queryRepository, Func<IQueryable<T>, IQueryable<T>> queryShaper, int pageIndex, int pageSize, CancellationToken cancellationToken = default) where T : class {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));

            if (queryShaper == null)
                throw new ArgumentNullException(nameof(queryShaper));

            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            var groups = await queryRepository.GetAsync(
                                q => {
                                    IQueryable<T> query = queryShaper(q);
                                    var startIndex = pageIndex * pageSize;
                                    return query.Skip(startIndex)
                                                .Take(pageSize)
                                                .GroupBy(
                                                    _ => new
                                                    {
                                                        Total = query.Count()
                                                    });
                                },
                                cancellationToken).ConfigureAwait(false);

            // return first group
            var result = groups.FirstOrDefault();

            if (result == null)
                return new PagedCollection<T>(Enumerable.Empty<T>(), 0L);

            return new PagedCollection<T>(result, result.Key.Total);
        }
    }
}