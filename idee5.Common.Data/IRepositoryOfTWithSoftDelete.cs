using System;

// Async is not neccessary, as the unit of work saves all changes.
namespace idee5.Common.Data;
/// <summary>
/// Adds two methods to handle soft (un)delete
/// </summary>
/// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
/// <typeparam name="TPrimaryKey">The <see cref="Type">type</see> of the primary key</typeparam>
public interface IRepositoryOfTWithSoftDelete<T, TPrimaryKey> : IRepository<T, TPrimaryKey>
    where T : class, ISoftDelete, IEntity<TPrimaryKey>
    where TPrimaryKey : notnull {
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