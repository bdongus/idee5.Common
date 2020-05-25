using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Common.Tests {

    public class TestCommandHandler : ICommandHandler<TestCommand> {

        // In real life create a constructor to inject dependencies https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=91
        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void Handle(TestCommand command)
        {
            if (command.Execute)
                command.Counter++;
        }
    }

    [TestClass]
    public class CommandHandlerTests {
        [UnitTest, TestMethod]
        public void CanUseCommandHandler()
        {
            var cmd = new TestCommand();
            cmd.Execute = false;
            var command = new TestCommandHandler();
            for (int i = 0; i < 3; i++)
                command.Handle(cmd);
            Assert.AreEqual(expected: 0, actual: cmd.Counter);

            cmd.Execute = true;
            command.Handle(cmd);

            Assert.AreEqual(expected: 1, actual: cmd.Counter);
        }
    }
}