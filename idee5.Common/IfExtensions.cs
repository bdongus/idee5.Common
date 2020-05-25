using System;

namespace idee5.Common {
    /// <summary>
    /// Extension methods for 'If' checking like checking if something is null or not null.
    /// </summary>
    public static class IfExtensions
    {
        /// <summary>
        /// The if not null.
        /// </summary>
        /// <param name="item">The item to be checked for <see langword="null"/>.</param>
        /// <param name="action">The <see cref="Action"/> to exectute it the <paramref name="item"/> is not <see langword="null"/>.</param>
        /// <typeparam name="TItem">The type ot the <paramref name="item"/>.</typeparam>
        public static void IfNotNull<TItem>(this TItem item, Action<TItem> action) where TItem : class {
            if (item != null) { action?.Invoke(item); }
        }

        /// <summary>
        /// Checks if the item is not null, and if so returns an action on that item, or a default value
        /// </summary>
        /// <typeparam name="TResult">the result type</typeparam>
        /// <typeparam name="TItem">The type</typeparam>
        /// <param name="item">The item to be checked for <see langword="null"/>.</param>
        /// <param name="func">The <see cref="Func{T, TResult}"/> to be executed on the item.</param>
        /// <param name="defaultValue">The default value to return if the <paramref name="item"/> is <see langword="null"/>.</param>
        public static TResult IfNotNull<TResult, TItem>(this TItem item, Func<TItem, TResult> func, TResult defaultValue = default)
        where TItem : class {
            return item != null && func != null ? func(item) : defaultValue;
        }
    }
}