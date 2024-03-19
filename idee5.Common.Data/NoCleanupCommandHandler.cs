using System.Threading.Tasks;
using System.Threading;

namespace idee5.Common.Data;

/// <summary>
/// The no cleanup command handler.
/// </summary>
public class NoCleanupCommandHandler : ICommandHandlerAsync<NoCleanupCommand> {
    /// <inheritdoc/>
    public Task HandleAsync(NoCleanupCommand command, CancellationToken cancellationToken = default) {
        return Task.CompletedTask;
    }
}
