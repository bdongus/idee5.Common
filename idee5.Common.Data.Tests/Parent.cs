using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace idee5.Common.Data.Tests {
    public class Parent : IValidatableObject {
        [Required(ErrorMessage = "Parent PropertyA is required")]
        [Range(0, 10, ErrorMessage = "Parent PropertyA not within range")]
        public int? PropertyA { get; set; }

        [Required(ErrorMessage = "Parent PropertyB is required")]
        [Range(0, 10, ErrorMessage = "Parent PropertyB not within range")]
        public int? PropertyB { get; set; }

        public Child Child { get; set; }

        [SkipRecursiveValidation]
        public Child SkippedChild { get; set; }

        public bool HasNoRealValidation { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (PropertyA.HasValue && PropertyB.HasValue && (PropertyA + PropertyB > 10))
                yield return new ValidationResult("Parent PropertyA and PropertyB cannot add up to more than 10");
        }
    }
}