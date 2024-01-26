#if !NETSTANDARD
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Telerik.JustMock;

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
        List<ServiceDescriptor> serviceDescriptors = [];
        var services = Mock.Create<IServiceCollection>();
        Mock.Arrange(() => services.Add(Arg.IsAny<ServiceDescriptor>())).DoInstead((ServiceDescriptor sd) => serviceDescriptors.Add(sd));
        // Act
        services.RegisterHandlers(typeof(ICommandHandlerAsync<>));

        // Assert
        Assert.AreEqual(4, serviceDescriptors.Count);
    }

    /// <summary>
    /// Can add asynchronous query handlers.
    /// </summary>
    [TestMethod]
    public void CanAddAsyncQueryHandlers() {
        // Arrange
        List<ServiceDescriptor> serviceDescriptors = [];
        var services = Mock.Create<IServiceCollection>();
        Mock.Arrange(() => services.Add(Arg.IsAny<ServiceDescriptor>())).DoInstead((ServiceDescriptor sd) => serviceDescriptors.Add(sd));
        // Act
        services.RegisterHandlers(typeof(IQueryHandlerAsync<,>));

        // Assert
        Assert.AreEqual(1, serviceDescriptors.Count);
    }
}

#endif