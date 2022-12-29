using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace idee5.Common {
    /// <summary>
    /// C# object extension methods.
    /// </summary>
    public static class ObjectExtensions {
        /// <summary>
        /// Cast an anonymous type to the specified type.
        /// </summary>
        /// <typeparam name="T">Generic type parameter. The specified type.</typeparam>
        /// <param name="obj">The object to cast.</param>
        /// <returns>The object as the specified type.</returns>
        public static T As<T>(this object obj) => (T)obj;

        /// <summary>
        /// Cast an anonymous type to the specified type or the type's default value.
        /// </summary>
        /// <typeparam name="T">Generic type parameter. The specified type.</typeparam>
        /// <param name="obj">The object to cast.</param>
        /// <returns>The object as the specified type.</returns>
        public static T AsOrDefault<T>(this object obj) {
            try {
                return (T)obj;
            }
            catch (InvalidCastException) {
                return default;
            }
        }

        /// <summary>
        /// Check if an <see cref="object"/> is of the given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">Object to check.</param>
        /// <param name="type">The type.</param>
        /// <returns>true if type of, false if not.</returns>
        public static bool IsTypeOf<T>(this T obj, Type type) => obj.GetType() == type;

        /// <summary>
        /// Gets an object's property value.
        /// </summary>
        /// <param name="o">This instance.</param>
        /// <param name="propertyName">NativeName of the property to read.</param>
        /// <returns>The property's value as object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="o"/> is <c>null</c>.</exception>
        public static object GetPropertyValue(this object o, string propertyName) {
            if (o == null)
                throw new ArgumentNullException(nameof(o));

            PropertyInfo propertyInfo = o.GetType().GetProperty(propertyName);
            return propertyInfo?.GetValue(o, null) ?? String.Empty;
        }

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
        /// <para>This overload uses generic <see cref="IEqualityComparer{T}"/> implementations to compare the items for the best performance.</para>
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
            int length;
            if (set == null || (length = set.Length) == 0)
                return false;
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

        /// <summary>
        /// Returns all properties with their value in a <see cref="string"/>.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <returns>The object properties and their values in a <see cref="string"/></returns>
        public static string AsString(this object value) => string.Join(Environment.NewLine, value.GetType().GetProperties().Select(prop => $"{prop.Name}: {prop.GetValue(value, null)}"));
    }
}