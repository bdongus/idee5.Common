using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    internal class TestCommandHandler : ICommandHandlerAsync<TestCommand> {
        public Task HandleAsync(TestCommand command, CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }
    }
}