namespace idee5.Common {
#pragma warning disable CA1040 // Avoid empty interfaces
    /// <summary>
    /// Defined to prevent casts of the return type in the consumer method.
    /// </summary>
    /// <typeparam name="TResult">Result data type.</typeparam>
    public interface IQuery<TResult>
#pragma warning restore CA1040 // Avoid empty interfaces
    {
    }
}