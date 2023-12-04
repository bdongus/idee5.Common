using System;
using System.Collections.Generic;

namespace idee5.Common;
/// <summary>
/// Extension methods for Collections.
/// </summary>
public static class CollectionExtensions {
    /// <summary>
    /// Adds an item to the <see cref="ICollection{T}"/> if it's not already in the collection.
    /// </summary>
    /// <param name="source">Collection</param>
    /// <param name="item">Item to check and add.</param>
    /// <typeparam name="T">Type of the items in the collection.</typeparam>
    /// <returns>Returns true if added, returns false if not.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static bool AddIfNotContains<T>(this ICollection<T> source, T item) {
#if NETSTANDARD2_0_OR_GREATER
        if (source == null) throw new ArgumentNullException(nameof(source));
#else
        ArgumentNullException.ThrowIfNull(source);
#endif
        if (source.Contains(item)) {
            return false;
        }

        source.Add(item);
        return true;
    }
}