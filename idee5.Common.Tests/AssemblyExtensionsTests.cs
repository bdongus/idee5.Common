using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace idee5.Common.Tests;
[TestClass]
public class AssemblyExtensionsTests {
    [TestMethod]
    public void CanGetImplementationsOfGenericInterface() {
        // Arrange
        var asms = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);
        // Act
        var result = asms.SelectMany(a => a.GetImplementationsWithoutDecorators(typeof(IQueryHandler<,>)));

        // Assert
        Assert.AreEqual(4, result.Count());
        Assert.IsTrue(result.Any(t => typeof(CountryQueryHandler) == t));
        Assert.IsTrue(result.Any(t => typeof(CurrenciesForISOCodesQueryHandler) == t));
        Assert.IsTrue(result.Any(t => typeof(LanguageQueryHandler) == t));
        Assert.IsTrue(result.Any(t => typeof(QueryHandler) == t));
    }

    [TestMethod]
    public void CanGetImplementationsWithoutDecorators() {
        // Arrange
        var asms = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);
        // Act
        var result = asms.SelectMany(a => a.GetImplementationsWithoutDecorators(typeof(ICommandHandlerAsync<>)));

        // Assert
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(t => typeof(RecordCommandHandlerAsync) == t));
        Assert.IsTrue(result.Any(t => typeof(TestCommandHandlerAsync) == t));
    }
    [TestMethod]
    public void CanGetSpecificImplementationsWithoutDecorators() {
        // Arrange
        var asms = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);

        // Act
        var result = asms.SelectMany(a => a.GetImplementationsWithoutDecorators<ICommandHandlerAsync<TestCommandRecord>>());

        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.Any(t => typeof(RecordCommandHandlerAsync) == t));
    }
}
