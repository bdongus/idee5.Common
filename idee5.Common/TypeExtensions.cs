using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace idee5.Common {
    public static class TypeExtensions {
        /// <summary>
        /// Gets all public constant values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns>The <see cref="IEnumerable{T}"/> with all constant values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public static IEnumerable<T> GetAllPublicConstantValues<T>(this Type type) {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
#pragma warning disable HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
#pragma warning restore HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
#pragma warning disable HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
                .Select(x => (T) x.GetValue(null));
#pragma warning restore HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
        }

        /// <summary>
        /// Check if the given type is a CLR type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>True if it is a CLR type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public static bool IsClrType(this Type type) {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            string fullname = type.Assembly.FullName;
            // check new (net core), then old (full framework) .net
            // Assuming more usage of .net core, we check that first.
            return fullname.StartsWith("System.Private.CoreLib", StringComparison.InvariantCulture)
                || fullname.StartsWith("mscorlib", StringComparison.InvariantCulture);
        }
    }
}