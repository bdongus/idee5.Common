using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace idee5.Common.Data
{
    /// <summary>
    /// Composite validation result. Contains validation results from the recursive validations.
    /// </summary>
    public class CompositeValidationResult : ValidationResult
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();

        /// <summary>
        /// List of multiple <see cref="ValidationResult"/>s.
        /// </summary>
        public IEnumerable<ValidationResult> Results {
            get {
                return _results;
            }
        }

        /// <inheritdoc />
        public CompositeValidationResult(string errorMessage) : base(errorMessage)
        {
        }

        /// <inheritdoc />
        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames)
        {
        }

        /// <inheritdoc />
        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult)
        {
        }

        /// <summary>
        /// Add a <see cref="ValidationResult"/> to the result list.
        /// </summary>
        /// <param name="validationResult"></param>
        public void AddResult(ValidationResult validationResult)
        {
            _results.Add(validationResult);
        }
    }
}