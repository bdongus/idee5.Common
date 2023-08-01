using System;
using System.ComponentModel.DataAnnotations;

namespace idee5.Common.Data;
/// <summary>
/// Check if the given string is a valid GS1 GTIN (EAN/UPC)
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ValidGTINAttribute : ValidationAttribute {
    /// <inheritdoc/>
    public override bool IsValid(object value) {
        if (value == null) {
            return true;
        }
        return value is string gln && (gln.IsNullOrEmpty()
            || gln.Length == 8
            || gln.Length == 12
            || gln.Length == 13
            || gln.Length == 14) && gln.IsValidGS1Id();
    }
}
