using System;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    public class TestEntityConsoleOutput : ICommandHandlerAsync<TestEntityResult>
	{
		public bool Executed { get; set; }
		public Task HandleAsync(TestEntityResult command, CancellationToken cancellationToken) {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command.Entities.ForEach(te => Console.WriteLine($"{te.Label} - {te.MasterSystemHierarchy} - {te.MasterSystemId}"));
            Executed = true;
            return Task.CompletedTask;
        }
    }
}
