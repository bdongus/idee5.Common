#if !NETSTANDARD
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Common.Data.Tests;
/// <summary>
/// The service collection tests.
/// </summary>
[TestClass]
public class ServiceCollectionTests {
    /// <summary>
    /// Can add asynchronous command handlers.
    /// </summary>
    [TestMethod]
    public void CanAddAsyncCommandHandlers() {
        // Arrange
        var services= new ServiceCollection();
        // Act
        services.RegisterHandlers(typeof(ICommandHandlerAsync<>));

        // Assert
        Assert.AreEqual(4, services.Count);
    }

    /// <summary>
    /// Can add asynchronous query handlers.
    /// </summary>
    [TestMethod]
    public void CanAddAsyncQueryHandlers() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterHandlers(typeof(IQueryHandlerAsync<,>));

        // Assert
        Assert.AreEqual(1, services.Count);
    }
}

#endif