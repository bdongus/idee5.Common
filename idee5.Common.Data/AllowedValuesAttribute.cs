using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace idee5.Common.Data {
    /// <summary>
    /// Base class for allowed values validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class AllowedValuesAttribute: ValidationAttribute {
        /// <summary>
        /// Retrieve the list of allowed values.
        /// </summary>
        /// <param name="validationContext">Context the validation is performed in.</param>
        /// <returns>Allowed values</returns>
        protected abstract object[] GetValues(ValidationContext validationContext);

        protected abstract string GetInvalidValueMessage(
            object invalidValue,
            object[] validValues);

        /// <inheritdoc/>
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext) {
            var values = GetValues(validationContext);
            var exists = values.Contains(value);

            if (exists) {
                return ValidationResult.Success;
            }

            var msg = GetInvalidValueMessage(value, values);
            return new ValidationResult(msg);
        }
    }
}
