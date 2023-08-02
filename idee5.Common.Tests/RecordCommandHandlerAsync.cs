using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Tests;
public class RecordCommandHandlerAsync : ICommandHandlerAsync<TestCommandRecord> {
    public Task HandleAsync(TestCommandRecord command, CancellationToken cancellationToken = default) {
        throw new System.NotImplementedException();
    }
}
