using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace idee5.Common.Data;
/// <inheritdoc/>
public class AsyncDataImporter<TQuery, TCommand, TCleanupCmd> : IAsyncDataImporter<TQuery, TCommand, TCleanupCmd> where TQuery : IQuery<TCommand> where TCleanupCmd : ICleanupCommand {
    protected readonly IAsyncInputHandler<TQuery, TCommand> inputHandler;
    protected readonly ICommandHandlerAsync<TCommand> outputHandler;
    private readonly ITimeProvider _timeProvider;
    protected readonly ICommandHandlerAsync<TCleanupCmd> cleanupHandler;
    private readonly ILogger<AsyncDataImporter<TQuery, TCommand, TCleanupCmd>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncDataImporter<,>"/> class
    /// </summary>
    /// <param name="inputHandler">The input handler feeding data into the <paramref name="outputHandler"/></param>
    /// <param name="outputHandler">The output handler</param>
    /// <param name="cleanupHandler">The cleanup handler</param>
    /// <param name="logger">Optional logger. If none is specified <see cref="NullLogger{T}"/></param> is used.
    /// <param name="timeProvider">Optional time provider. If none is specified, <see cref="DefaultTimeProvider"/> is used.</param>
    public AsyncDataImporter(IAsyncInputHandler<TQuery, TCommand> inputHandler, ICommandHandlerAsync<TCommand> outputHandler,
        ICommandHandlerAsync<TCleanupCmd> cleanupHandler, ILogger<AsyncDataImporter<TQuery, TCommand, TCleanupCmd>>? logger = null, ITimeProvider? timeProvider = null) {
        this.inputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));
        this.outputHandler = outputHandler ?? throw new ArgumentNullException(nameof(outputHandler));
        _timeProvider = timeProvider ?? new DefaultTimeProvider();
        this.cleanupHandler = cleanupHandler ?? throw new ArgumentNullException(nameof(cleanupHandler));
        _logger = logger ?? NullLogger<AsyncDataImporter<TQuery, TCommand, TCleanupCmd>>.Instance;
    }
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> or <paramref name="cleanupCmd"/> is <c>NULL</c></exception>
    public virtual async Task ExecuteAsync(TQuery query, TCleanupCmd cleanupCmd, CancellationToken cancellationToken = default) {
#if NETSTANDARD2_0_OR_GREATER
        if (query == null) throw new ArgumentNullException(nameof(query));
        if (cleanupCmd == null) throw new ArgumentNullException(nameof(cleanupCmd));
#else
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(cleanupCmd);
#endif
        DateTime importStartedAt = _timeProvider.UtcNow;
        int importCount = 0;
        _logger.ImportStarted(query.GetType().Name, importStartedAt);
        await foreach (TCommand item in inputHandler.HandleAsync(query, cancellationToken)) {
            if (item != null) {
                importCount++;
                _logger.Importing(importCount, item.AsString());
                await outputHandler.HandleAsync(item, cancellationToken).ConfigureAwait(false);
            }
        }
        _logger.CleaningUpImport(cleanupCmd.GetType().Name, _timeProvider.UtcNow);
        cleanupCmd.BeforeUTC = importStartedAt;

        await cleanupHandler.HandleAsync(cleanupCmd, cancellationToken).ConfigureAwait(false);
        _logger.ImportFinished(query.GetType().Name, _timeProvider.UtcNow, importCount);
    }
}
