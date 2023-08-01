using System;
using System.Collections.Generic;
using System.Dynamic;

namespace idee5.Common {
    /// <summary>
    /// Extension methods for <see cref="ExpandoObject"/>s.
    /// </summary>
    public static class ExpandoExtensions {
        #region Public Methods

        /// <summary>
        /// Add a property to an <see cref="ExpandoObject"/>.
        /// </summary>
        /// <param name="expando">This instance.</param>
        /// <param name="propertyName">NativeName of the property to add.</param>
        /// <param name="propertyValue">Value of the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expando"/> is <c>null</c>.</exception>
        public static void AddProperty(this ExpandoObject expando, string propertyName, object propertyValue) {
            if (expando == null)
                throw new ArgumentNullException(nameof(expando));
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (!expandoDict.ContainsKey(propertyName)) {
                // Add it only if it doesn't exist yet
                expandoDict.Add(propertyName, propertyValue);
            }
        }

        /// <summary>
        /// Get an <see cref="ExpandoObject"/>'s property value by the property name.
        /// </summary>
        /// <param name="expando">This instance.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The property's value as <see cref="object"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expando"/> is <c>null</c>.</exception>
        public static object GetPropertyByName(this ExpandoObject expando, string propertyName) {
            if (expando == null)
                throw new ArgumentNullException(nameof(expando));

            var dict = (IDictionary<string, object>)expando;
            if (dict.ContainsKey(propertyName))
                return dict[propertyName];
            else return null;
        }

        #endregion Public Methods
    }
}