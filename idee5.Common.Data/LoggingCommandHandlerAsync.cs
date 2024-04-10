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
/// <typeparam commandName="TCommand">Type of the commad parameters</typeparam>
public class LoggingCommandHandlerAsync<TCommand> : ICommandHandlerAsync<TCommand> {
    private readonly ICommandHandlerAsync<TCommand> _handler;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<TCommand> _logger;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param commandName="handler">The decorated handler.</param>
    /// <param commandName="loggerFactory">Factory creating the logger to use.</param>
    public LoggingCommandHandlerAsync(ICommandHandlerAsync<TCommand> handler, ILoggerFactory loggerFactory) {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<TCommand>();
    }

    /// <inheritdoc/>
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default) {
#if NETSTANDARD2_0_OR_GREATER
        if (command == null) throw new ArgumentNullException(nameof(command));
#else
        ArgumentNullException.ThrowIfNull(command);
#endif
        string commandName = typeof(TCommand).Name;
        _logger.InvokingCommand(commandName);
        _logger.CommandParametersAre(Environment.NewLine + command.AsString());
        await _handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        _logger.CommandExecuted(commandName);
    }
}
