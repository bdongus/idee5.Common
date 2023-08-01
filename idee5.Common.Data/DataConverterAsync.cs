namespace idee5.Common.Data;
/// <inheritdoc />
public class DataConverterAsync<TInput, TIntermediate> : ADataConverterAsync<TInput, TIntermediate> where TInput : IQuery<TIntermediate> {
    /// <summary>
    /// Create a new <see cref="DataConverterAsync{TInput, TIntermediate}"/>.
    /// </summary>
    /// <param name="inputHandler">Input query handler.</param>
    /// <param name="outputHandler">Output command handler.</param>
    public DataConverterAsync(IQueryHandlerAsync<TInput, TIntermediate> inputHandler, ICommandHandlerAsync<TIntermediate> outputHandler)
        : base(inputHandler, outputHandler) {
    }
}