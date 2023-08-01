using System;
using System.ComponentModel.DataAnnotations;

namespace idee5.Common.Data;
/// <summary>
/// Check if the given string is a valid GS1 Global Location Number
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ValidGLNAttribute : ValidationAttribute {
    /// <inheritdoc/>
    public override bool IsValid(object value) => value == null || (value is string gln && (gln.IsNullOrEmpty() || gln.Length == 13) && gln.IsValidGS1Id());
}
