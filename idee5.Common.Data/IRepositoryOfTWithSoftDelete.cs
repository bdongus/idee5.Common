using System;

// https://blogs.msdn.microsoft.com/mrtechnocal/2014/03/16/asynchronous-repositories/
// Async is not neccessary, as the unit of work saves all changes.
namespace idee5.Common.Data {
    /// <summary>
    /// Defines the behavior of a repository of items. Used for simple CRUD operations.
    /// Use <see cref="ICommandHandlerAsync{TCommand}"/> or <see cref="IQueryHandler{TQuery, TResult}"/> for complex scenarios.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
    public interface IRepositoryOfTWithSoftDelete<T, TKey> : IRepository<T, TKey> where T : class, ISoftDelete, IEntity<TKey> {
        /// <summary>
        /// Marks an item as deleted.
        /// </summary>
        /// <param name="item">The item to soft delete.</param>
         /// <returns>The modified item.</returns>
        T MarkAsDeleted(T item);

        /// <summary>
        /// Undelete an for deletion marked item,
        /// </summary>
        /// <param name="item">The item to undelete.</param>
        /// <returns>The modified item.</returns>
        T Undelete(T item);
    }
}