using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class DataConverterTests {
        [UnitTest, TestMethod]
        public async Task CanConvertSimpleListAsync() {
            // Arrange
            var cancellationToken = new CancellationToken();
            var outputHandler = new TestEntityConsoleOutput();
            var converter = new DataConverterAsync<InputQuery, TestEntityResult>(new TestEntityInputHandler(), outputHandler);
            // Act
            await converter.ExecuteAsync(new InputQuery() { MasterSystemHierarchy = "001" }, cancellationToken).ConfigureAwait(false);
            // Assert
            Assert.IsTrue(outputHandler.Executed);
        }

        [IntegrationTest, TestMethod]
        public async Task CanFailValidateOutputHandler() {
            // Arrange
            var cancellationToken = new CancellationToken();
            var command = new TestEntityConsoleOutput();
            var recursiveAnnotationsValidator = new RecursiveAnnotationsValidator();
            var reporter = new ConsoleValidationReporter();

            var outputHandler = new DataAnnotationValidationCommandHandlerAsync<TestEntityResult>(recursiveAnnotationsValidator, reporter, command);

            var converter = new DataConverterAsync<InputQuery, TestEntityResult>(new TestEntityInputHandler(), outputHandler);
            // Act
            await converter.ExecuteAsync(new InputQuery() { MasterSystemHierarchy = "002" }, cancellationToken).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(command.Executed);
            Assert.AreEqual(1, outputHandler.ValidationResults.Count);
            Assert.AreEqual("The Label field is required.", outputHandler.ValidationResults[0].ErrorMessage);
        }
    }
}