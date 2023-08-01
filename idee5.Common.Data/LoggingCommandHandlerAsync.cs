using idee5.Common.Data.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// <see cref="ICommandHandlerAsync{TCommand}"/> decorator logging the invocation to "Trace"
/// and the parameters to "Info".
/// </summary>
/// <typeparam name="TCommand">Type of the commad parameters</typeparam>
public class LoggingCommandHandlerAsync<TCommand> : ICommandHandlerAsync<TCommand> {
    private readonly ICommandHandlerAsync<TCommand> _handler;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<TCommand> _logger;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="handler">The decorated handler.</param>
    /// <param name="loggerFactory">Factory creating the logger to use.</param>
    public LoggingCommandHandlerAsync(ICommandHandlerAsync<TCommand> handler, ILoggerFactory loggerFactory) {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<TCommand>();
    }

    /// <inheritdoc/>
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default) {
        _logger.LogTrace(Resources.InvokingComand, typeof(TCommand).Name);
        _logger.LogInformation(Resources.CommandParametersAre, Environment.NewLine + command.AsString());
        await _handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        _logger.LogTrace(Resources.CommandInvoked, typeof(TCommand).Name);
    }
}
