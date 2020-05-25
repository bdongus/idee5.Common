using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace idee5.Common
{
    /// <summary>
    /// C# object extension methods.
    /// </summary>
    public static class ObjectExtensions {
        /// <summary>
        /// Gets an object's property value.
        /// </summary>
        /// <param name="o">This instance.</param>
        /// <param name="propertyName">Name of the property to read.</param>
        /// <returns>The property's value as object or <see langword="null"/> if <paramref name="o"/> is <see langword="null"/>
        /// or the property does not exist.</returns>
        public static object GetPropertyValue(this object o, string propertyName)
            => o?.GetType().GetProperty(propertyName)?.GetValue(o, null);

        /// <summary>
        /// Gets whether <paramref name="item"/> is among the elements of <paramref name="set"/>.
        /// <br/>See the <strong>Examples</strong> section for an example.
        /// </summary>
        /// <param name="item">The item to search for in <paramref name="set"/>.</param>
        /// <param name="set">The set of items in which to search the specified <paramref name="item"/>.</param>
        /// <typeparam name="T">The type of <paramref name="item"/> and the <paramref name="set"/> elements.</typeparam>
        /// <returns><see langword="true"/> if <paramref name="item"/> is among the elements of <paramref name="set"/>; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// <para>This method works similarly to the <c>in</c> operator in SQL and Pascal.</para>
        /// <para>This overload uses generic <see cref="IEqualityComparer{T}"/> implementations to compare the items for the best performance.
        /// </remarks>
        /// <example>
        /// <code lang="C#"><![CDATA[
        ///     string stringValue = "blah";
        ///
        ///     // standard way:
        ///     if (stringValue == "something" || stringValue == "something else" || stringValue == "or...")
        ///         DoSomething();
        ///
        ///     // In method:
        ///     if (stringValue.In("something", "something else", "or..."))
        ///         DoSomething();
        /// ]]></code>
        /// </example>
        public static bool In<T>(this T item, params T[] set) {
            int length = set?.Length ?? 0;
            // special case enum comparison
            IEqualityComparer<T> comparer = item is Enum ? (IEqualityComparer<T>) EnumComparer<T>.Instance : EqualityComparer<T>.Default;
            for (int i = 0; i < length; i++) {
                if (comparer.Equals(item, set[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Try to convert <paramref name="value"/> to the destination type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Destination type.</typeparam>
        /// <param name="value">Value to convert.</param>
        /// <param name="result">Converted value. Destination default value if the conversion failed.</param>
        /// <returns><c>True</c> if the conversion succeeded.</returns>
        public static bool TryConvert<T>(this object value, out T result) {
            // default output value
            result = default;

            if (value == null || value == DBNull.Value) return false;

            // if source and destination type is the same, just use it
            if (typeof(T) == value.GetType()) {
                result = (T) value;
                return true;
            }

            string typeName = typeof(T).Name;

            try {
                // if the type is nullable or an Enum, use the TypeDescriptor and TypeConverter
                if (typeName.IndexOf(typeof(Nullable).Name, StringComparison.Ordinal) > -1 ||
                    typeof(T).BaseType.Name.IndexOf(typeof(Enum).Name, StringComparison.Ordinal) > -1) {
                    TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
                    result = (T) tc.ConvertFrom(value);
                } else { result = (T) Convert.ChangeType(value, typeof(T)); }
            }
            catch { return false; }

            return true;
        }
    }
}