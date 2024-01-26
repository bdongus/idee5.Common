using System.Threading.Tasks;
using System.Threading;

namespace idee5.Common.Data.Tests;

public class TestCleanupCommandHandler : ICommandHandlerAsync<TestCleanupCommand> {
    public bool Executed { get; private set; }
    /// <inheritdoc/>
    public Task HandleAsync(TestCleanupCommand command, CancellationToken cancellationToken = default) {
        Executed = true;
        return Task.CompletedTask;
    }
}
