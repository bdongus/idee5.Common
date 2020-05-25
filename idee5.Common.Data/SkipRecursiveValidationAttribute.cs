using System;

namespace idee5.Common.Data {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SkipRecursiveValidationAttribute : Attribute
    {
    }
}