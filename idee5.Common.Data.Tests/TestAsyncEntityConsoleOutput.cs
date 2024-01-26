using System;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests;

public class TestAsyncEntityConsoleOutput : ICommandHandlerAsync<TestEntity> {
    public bool Executed { get; set; }
    public Task HandleAsync(TestEntity command, CancellationToken cancellationToken) {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        Console.WriteLine($"{command.Label} - {command.MasterSystemHierarchy} - {command.MasterSystemId}");
        Executed = true;
        return Task.CompletedTask;
    }
}
