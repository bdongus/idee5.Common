using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

// Taken from https://github.com/reustmd/DataAnnotationsValidatorRecursive and ported to netstandard2.0
namespace idee5.Common.Data {
    public class RecursiveAnnotationsValidator : IRecursiveAnnotationsValidator {
        /// <inheritdoc />
        public bool TryValidateObject(object obj, ICollection<ValidationResult> results, IDictionary<object, object>? validationContextItems = null)
            => Validator.TryValidateObject(obj, new ValidationContext(obj, null, validationContextItems), results, true);

        /// <inheritdoc />
        public bool TryValidateObjectRecursive<T>(T obj, IList<ValidationResult> results, IDictionary<object, object>? validationContextItems = null)
            => TryValidateObjectRecursive(obj, results ?? throw new ArgumentNullException(nameof(results)), new HashSet<object>(), validationContextItems);

        private bool TryValidateObjectRecursive<T>(T obj, IList<ValidationResult> results, ISet<object> validatedObjects, IDictionary<object, object>? validationContextItems = null) {
            if (obj == null) return false;
            // avoid infinite loops on cyclical object graphs
            if (validatedObjects.Contains(obj)) {
                return true;
            }

            validatedObjects.Add(obj);
            bool result = TryValidateObject(obj, results, validationContextItems);
            var properties = obj.GetType().GetProperties().Where(prop => prop.CanRead
                && prop.GetCustomAttributes(typeof(SkipRecursiveValidationAttribute), false).Length == 0
                && prop.GetIndexParameters().Length == 0).ToList();
            for (int i = 0; i < properties.Count; i++) {
                PropertyInfo propertyInfo = properties[i];
                if (propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType.IsValueType) continue;

                var value = obj.GetPropertyValue(propertyInfo.Name);

                if (value == null) continue;

                if (value is IEnumerable asEnumerable) {
                    foreach (var enumObj in asEnumerable) {
                        var nestedResults = new List<ValidationResult>();
                        if (!TryValidateObjectRecursive(enumObj, nestedResults, validatedObjects, validationContextItems)) {
                            result = false;
                            foreach (ValidationResult validationResult in nestedResults) {
                                results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => propertyInfo.Name + '.' + x)));
                            }
                        }
                    }
                }
                else {
                    var nestedResults = new List<ValidationResult>();
                    if (!TryValidateObjectRecursive(value, nestedResults, validatedObjects, validationContextItems)) {
                        result = false;
                        foreach (ValidationResult validationResult in nestedResults) {
                            results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => propertyInfo.Name + '.' + x)));
                        }
                    }
                }
            }

            return result;
        }
    }
}