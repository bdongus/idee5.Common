using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Tests {
    public class TestCommandHandlerAsync : ICommandHandlerAsync<TestCommand> {
        // In real life create a constructor to inject dependencies https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=91
        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command.</param>

        public Task HandleAsync(TestCommand command, CancellationToken cancellationToken) {
            if (command.Execute)
                command.Counter++;
            return Task.CompletedTask;
        }
    }

    [TestClass]
    public class CommandHandlerAsyncTests {
        [UnitTest, TestMethod]
        public async Task CanUseCommandHandlerAsync() {
            var cmd = new TestCommand {
                Execute = false
            };
            var command = new TestCommandHandlerAsync();
            for (int i = 0; i < 3; i++)
                await command.HandleAsync(cmd, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(expected: 0, actual: cmd.Counter);

            cmd.Execute = true;
            await command.HandleAsync(cmd, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(expected: 1, actual: cmd.Counter);
        }
        [UnitTest, TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task CanUseRecordAsCommandAsync() {
            var cmd = new TestCommandRecord(42);
            var command = new RecordCommandHandlerAsync();

            await command.HandleAsync(cmd, CancellationToken.None).ConfigureAwait(false);

        }
    }
}