using System;
using System.Globalization;
using System.Reflection;

namespace idee5.Common {
    /// <summary>
    /// Reflection helper methods.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Creates an instance of a type based on a string.
        /// </summary>
        /// <param name="typeName">String representation of the <see cref="Type"/>.</param>
        /// <param name="args">Optional constructor parameters.</param>
        /// <returns>The created instance or null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="typeName"/> is <c>null</c>.</exception>
        public static object CreateInstanceFromString(string typeName, params object[] args) {
            if (typeName == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(typeName));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            object instance = null;
            try {
                Type type = GetTypeFromName(typeName);
                if (type != null)
                    instance = Activator.CreateInstance(type, args);
            }
            catch (Exception) { throw; }

            return instance;
        }

        /// <summary>
        /// Helper routine that looks up a type name and tries to retrieve the full type reference in
        /// the actively executing assemblies.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns>The found <see cref="Type"/> or <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="typeName"/> is <c>null</c>.</exception>
        public static Type GetTypeFromName(string typeName) {
            if (typeName == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(typeName));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            var type = Type.GetType(typeName, throwOnError: false);
            if (type != null)
                return type;

            // try to find manually
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
                type = asm.GetType(typeName, throwOnError: false);

                if (type != null)
                    break;
            }
            return type;
        }

        /// <summary>
        /// Retrieves a value from a static property by specifying a type full name and property
        /// </summary>
        /// <param name="typeName">Full type name (namespace.class)</param>
        /// <param name="property">Property to get value from</param>
        /// <returns>The property value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="typeName"/> or <paramref name="property"/> is <c>null</c>.</exception>
        public static object GetStaticProperty(string typeName, string property) {
            if (typeName == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(typeName));

            if (property == null)
                throw new ArgumentNullException(nameof(property));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            Type type = GetTypeFromName(typeName);
            if (type == null)
                return null;

            return GetStaticProperty(type, property);
        }

        /// <summary>
        /// Returns a static property from a given type
        /// </summary>
        /// <param name="type">Type instance for the static property</param>
        /// <param name="property">Property name as a string</param>
        /// <returns>The property value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="property"/> is <c>null</c>.</exception>
        public static object GetStaticProperty(Type type, string property) {
            if (type == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(type));

            if (property == null)
                throw new ArgumentNullException(nameof(property));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            object result;
            try { result = type.InvokeMember(
                property,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty,
                binder: null,
                target: type,
                args: null,
                CultureInfo.CurrentCulture); }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { return null; }
#pragma warning restore CA1031 // Do not catch general exception types

            return result;
        }
    }
}