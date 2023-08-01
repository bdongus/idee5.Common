using System;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// Base class for generic and specific data conversions.
/// </summary>
/// <typeparam name="TInput">Input handler parameters.</typeparam>
/// <typeparam name="TIntermediate">Intermediate data type.
/// The input handler returns this type and the output handler takes it to produce the desired output.
/// This split is done for better reusability of both handlers.
/// </typeparam>
public abstract class ADataConverterAsync<TInput, TIntermediate> where TInput : IQuery<TIntermediate> {
    /// <summary>
    /// <see cref="IQueryHandlerAsync{TQuery, TResult}"/> providing the input for the <see cref="_outputHandler"/>.
    /// </summary>
    private readonly IQueryHandlerAsync<TInput, TIntermediate> _inputHandler;

    /// <summary>
    /// <see cref="ICommandHandlerAsync{TCommand}"/> using the input handler's output to convert the data to its output format.
    /// Use to access validation results.
    /// </summary>
    private readonly ICommandHandlerAsync<TIntermediate> _outputHandler;

    /// <summary>
    /// Base constructor
    /// </summary>
    /// <param name="inputHandler">Input handler, creating the intermediate result.</param>
    /// <param name="outputHandler">Output handler converting the intermediate result.</param>
    protected ADataConverterAsync(IQueryHandlerAsync<TInput, TIntermediate> inputHandler, ICommandHandlerAsync<TIntermediate> outputHandler) {
        _outputHandler = outputHandler ?? throw new ArgumentNullException(nameof(outputHandler));
        _inputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));
    }

    /// <summary>
    /// Execute the data conversion.
    /// </summary>
    /// <returns>The execute.</returns>
    /// <param name="query">The query/filter parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public virtual async Task ExecuteAsync(TInput query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));
        TIntermediate pocos = await _inputHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        await _outputHandler.HandleAsync(pocos, cancellationToken).ConfigureAwait(false);
    }
}