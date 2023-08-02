using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Tests;

public class DecoratedRecordCommandHandlerAsync : ICommandHandlerAsync<TestCommandRecord> {
    private readonly ICommandHandlerAsync<TestCommandRecord> decoratee;

    public DecoratedRecordCommandHandlerAsync(ICommandHandlerAsync<TestCommandRecord> decoratee)
    {
        this.decoratee = decoratee;
    }
    public Task HandleAsync(TestCommandRecord command, CancellationToken cancellationToken = default) {
        return decoratee.HandleAsync(command, cancellationToken);
    }
}