using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace idee5.Common.Data {
    public interface IRecursiveAnnotationsValidator {
        /// <summary>
        /// Tries to validate an object. Used for deep validation of nested classes.
        /// </summary>
        /// <returns><c>true</c>, if validatation was successful, <c>false</c> otherwise.</returns>
        /// <param name="instance">The object to be validated.</param>
        /// <param name="results">The validation results.</param>
        /// <param name="validationContextItems">Validation context items.</param>
        bool TryValidateObject(object instance, ICollection<ValidationResult> results, IDictionary<object, object> validationContextItems = null);

        /// <summary>
        /// Tries the validate this object recursively.
        /// </summary>
        /// <returns><c>true</c>, if validatation was successful, <c>false</c> otherwise.</returns>
        /// <param name="instance">The object to be validated.</param>
        /// <param name="results">The validation results.</param>
        /// <param name="validationContextItems">Validation context items.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        bool TryValidateObjectRecursive<T>(T instance, IList<ValidationResult> results, IDictionary<object, object> validationContextItems = null);
    }
}