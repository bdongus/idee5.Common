using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

// https://blogs.msdn.microsoft.com/mrtechnocal/2014/03/16/asynchronous-repositories/
namespace idee5.Common.Data {
    /// <summary>
    /// Provides extension methods for the <see cref="IQueryRepository{T}"/> interface.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Retrieves all items in the repository satisfied by the specified query asynchronously.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
        /// <param name="queryRepository">The extended <see cref="IQueryRepository{T}">repository</see>.</param>
        /// <param name="func">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IEnumerable{T}">sequence</see>
        /// of <typeparamref name="T">items</typeparamref>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support.")]
        public static Task<IEnumerable<T>> GetAsync<T>(this IQueryRepository<T> queryRepository, Func<IQueryable<T>, IEnumerable<T>> func, CancellationToken cancellationToken = default) where T : class
        {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return queryRepository.GetAsync(func, cancellationToken);
        }

        /// <summary>
        /// Retrieves a query result asynchronously.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
        /// <typeparam name="TResult">The <see cref="Type">type</see> of result to retrieve.</typeparam>
        /// <param name="queryRepository">The extended <see cref="IQueryRepository{T}">repository</see>.</param>
        /// <param name="func">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <typeparamref name="TResult">result</typeparamref> of the operation.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support.")]
        public static Task<TResult> GetAsync<T, TResult>(this IQueryRepository<T> queryRepository, Func<IQueryable<T>, TResult> func, CancellationToken cancellationToken = default) where T : class
        {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return queryRepository.GetAsync(func, cancellationToken);
        }

        /// <summary>
        /// Retrieves all items in the repository asynchronously.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
        /// <param name="queryRepository">The extended <see cref="IQueryRepository{T}">repository</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="IEnumerable{T}">sequence</see>
        /// of all <typeparamref name="T">items</typeparamref> in the repository.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support.")]
        public static Task<IEnumerable<T>> GetAllAsync<T>(this IQueryRepository<T> queryRepository, CancellationToken cancellationToken = default) where T : class
        {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));

#pragma warning disable HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
            return queryRepository.GetAsync<IEnumerable<T>>(q => q, cancellationToken);
#pragma warning restore HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
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
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support.")]
#pragma warning disable HAA0302 // Display class allocation to capture closure
        public static Task<IEnumerable<T>> FindByAsync<T>(this IQueryRepository<T> queryRepository, Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) where T : class
#pragma warning restore HAA0302 // Display class allocation to capture closure
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));

#pragma warning disable HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
#pragma warning disable HAA0301 // Closure Allocation Source
            return queryRepository.GetAsync<IEnumerable<T>>(q => q.Where(expression), cancellationToken);
#pragma warning restore HAA0301 // Closure Allocation Source
#pragma warning restore HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
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
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support.")]
#pragma warning disable HAA0302 // Display class allocation to capture closure
        public static Task<T> GetSingleAsync<T>(this IQueryRepository<T> queryRepository, Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) where T : class
#pragma warning restore HAA0302 // Display class allocation to capture closure
        {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

#pragma warning disable HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
#pragma warning disable HAA0301 // Closure Allocation Source
            return queryRepository.GetAsync(q => q.SingleOrDefault(expression), cancellationToken);
#pragma warning restore HAA0301 // Closure Allocation Source
#pragma warning restore HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
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
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support.")]
#pragma warning disable HAA0302 // Display class allocation to capture closure
        public static async Task<PagedCollection<T>> PaginateAsync<T>(this IQueryRepository<T> queryRepository, int pageIndex, int pageSize, CancellationToken cancellationToken = default) where T : class
#pragma warning restore HAA0302 // Display class allocation to capture closure
        {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            var groups = await queryRepository.GetAsync(
#pragma warning disable HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
#pragma warning disable HAA0301 // Closure Allocation Source
#pragma warning disable HAA0302 // Display class allocation to capture closure
                q =>
#pragma warning restore HAA0302 // Display class allocation to capture closure
#pragma warning restore HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
                {
                    var startIndex = pageIndex * pageSize;
                    return q.Skip(startIndex)
                            .Take(pageSize)
                            .GroupBy(
                                _ => new
#pragma warning restore HAA0301 // Closure Allocation Source
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
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support.")]
#pragma warning disable HAA0302 // Display class allocation to capture closure
        public static async Task<PagedCollection<T>> PaginateAsync<T>(this IQueryRepository<T> queryRepository, Func<IQueryable<T>, IQueryable<T>> queryShaper, int pageIndex, int pageSize, CancellationToken cancellationToken = default) where T : class
#pragma warning restore HAA0302 // Display class allocation to capture closure
        {
            if (queryRepository == null)
                throw new ArgumentNullException(nameof(queryRepository));

            if (queryShaper == null)
                throw new ArgumentNullException(nameof(queryShaper));

            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            var groups = await queryRepository.GetAsync(
#pragma warning disable HAA0301 // Closure Allocation Source
#pragma warning disable HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
                                q =>
#pragma warning restore HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
                                {
#pragma warning disable HAA0302 // Display class allocation to capture closure
                                    IQueryable<T> query = queryShaper(q);
#pragma warning restore HAA0302 // Display class allocation to capture closure
                                    var startIndex = pageIndex * pageSize;
                                    return query.Skip(startIndex)
                                                .Take(pageSize)
                                                .GroupBy(
                                                    _ => new
#pragma warning restore HAA0301 // Closure Allocation Source
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