using System;
using System.Collections.Generic;
using System.Linq;

namespace idee5.Common.Data;

/// <summary>
/// The data page extensions
/// </summary>
public static class DataPageExtensions {
    /// <summary>
    /// Apply paging to in memory collections
    /// </summary>
    /// <typeparam name="T">Type of the values</typeparam>
    /// <param name="values">The values</param>
    /// <param name="paging">The paging to use</param>
    /// <returns>A DataPage</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paging"/> is <c>null</c>.</exception>
    public static DataPage<T> Page<T>(this IEnumerable<T> values, PageInfo paging) {
#if NETSTANDARD2_0_OR_GREATER
        if (paging == null) throw new ArgumentNullException(nameof(paging));
#else
        ArgumentNullException.ThrowIfNull(paging);
#endif

        return new DataPage<T>() { Data = values.Skip(paging.PageIndex * paging.PageSize).Take(paging.PageSize).ToArray(), Paging = paging };
    }
    /// <summary>
    /// Apply paging to (database) queries
    /// </summary>
    /// <typeparam name="T">Type of the values</typeparam>
    /// <param name="values">The values</param>
    /// <param name="paging">The paging to use</param>
    /// <returns>A DataPage</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paging"/> is <c>null</c>.</exception>
    public static DataPage<T> Page<T>(this IQueryable<T> values, PageInfo paging) {
#if NETSTANDARD2_0_OR_GREATER
        if (paging == null) throw new ArgumentNullException(nameof(paging));
#else
        ArgumentNullException.ThrowIfNull(paging);
#endif

        IQueryable<T> items = values.Skip(paging.PageIndex * paging.PageSize).Take(paging.PageSize);
        return new DataPage<T>() { Data = items.ToArray(), Paging = paging };
    }
}