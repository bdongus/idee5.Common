using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MELT;
using System.Threading.Tasks;
using System.Linq;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class LoggingCommandHandlerAsyncTests {
        [TestMethod]
        public async Task CanLogCommandProperties() {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var loggerCreator = new TestLoggerCreator(loggerFactory);
            ICommandHandlerAsync<TestCommand> innerHandler = new TestCommandHandler();
            var handler = new LoggingCommandHandlerAsync<TestCommand>(innerHandler, loggerCreator);

            TestCommand testCommand = new TestCommand() {
                DecimalNum = 0.815M,
                DoubleNum =47.12,
                FloatNum = 47.11f,
                Id = 42,
                IsSomething = true,
                Text = "The answer is",
                UUID = Guid.Empty
            };

            // Act
            await handler.HandleAsync(testCommand).ConfigureAwait(false);

            // Assert
            var msgText = String.Format("Command parameters are : {0}", Environment.NewLine + testCommand.AsString());
            Assert.AreEqual(3, loggerFactory.Sink.LogEntries.Count());
            Assert.AreEqual(msgText, loggerFactory.Sink.LogEntries.ElementAt(1).Message);
        }

        [TestMethod]
        public async Task CanHandleNullProperties() {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var loggerCreator = new TestLoggerCreator(loggerFactory);
            ICommandHandlerAsync<TestCommand> innerHandler = new TestCommandHandler();
            var handler = new LoggingCommandHandlerAsync<TestCommand>(innerHandler, loggerCreator);

            TestCommand testCommand = new TestCommand() {
                DecimalNum = 0.815M,
                DoubleNum =47.12,
                FloatNum = 47.11f,
                Id = 42,
                IsSomething = true,
                Text = null,
                UUID = Guid.Empty
            };

            // Act
            await handler.HandleAsync(testCommand).ConfigureAwait(false);

            // Assert
            var msgText = String.Format("Command parameters are : {0}", Environment.NewLine + testCommand.AsString());
            Assert.AreEqual(3, loggerFactory.Sink.LogEntries.Count());
            Assert.AreEqual(msgText, loggerFactory.Sink.LogEntries.ElementAt(1).Message);
        }
    }
}