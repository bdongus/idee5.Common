using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests;

[TestClass]
public class AsyncDataImporterTests {
    [UnitTest, TestMethod]
    public async Task CanConvertSimpleListAsync() {
        // Arrange
        var inputHandler = new TestAsyncInputHandler();
        var outputHandler = new TestAsyncEntityConsoleOutput();
        var cleanupHandler = new TestCleanupCommandHandler();
        var cleanupCmd = new TestCleanupCommand();
        var importer = new AsyncDataImporter<TestInputQuery, TestEntity, TestCleanupCommand>(inputHandler, outputHandler, cleanupHandler, null, null);
        // Act
        await importer.ExecuteAsync(new TestInputQuery() { MasterSystemHierarchy = "001" }, cleanupCmd).ConfigureAwait(false);
        // Assert
        Assert.IsTrue(outputHandler.Executed);
        Assert.IsTrue(cleanupHandler.Executed);
    }

    [IntegrationTest, TestMethod]
    public async Task CanFailValidateOutputHandler() {
        // Arrange
        var inputHandler = new TestAsyncInputHandler();
        var command = new TestAsyncEntityConsoleOutput();
        var cleanupHandler = new TestCleanupCommandHandler();
        var cleanupCmd = new TestCleanupCommand();

        var recursiveAnnotationsValidator = new RecursiveAnnotationsValidator();
        var reporter = new ConsoleValidationReporter();

        var outputHandler = new DataAnnotationValidationCommandHandlerAsync<TestEntity>(recursiveAnnotationsValidator, reporter, command);

        var importer = new AsyncDataImporter<TestInputQuery, TestEntity, TestCleanupCommand>(inputHandler, outputHandler, cleanupHandler, null, null);
        // Act
        await importer.ExecuteAsync(new TestInputQuery() { MasterSystemHierarchy = "002" }, cleanupCmd).ConfigureAwait(false);

        // Assert
        Assert.IsFalse(command.Executed);
        Assert.IsTrue(cleanupHandler.Executed);
        Assert.AreEqual(1, outputHandler.ValidationResults.Count);
        Assert.AreEqual("The Label field is required.", outputHandler.ValidationResults[0].ErrorMessage);
    }
}
