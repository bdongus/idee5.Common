using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Common.Data.Tests;
[TestClass]
public class MasterSystemFormatterTests {
    [TestMethod, UnitTest]
    public void CanFormatDefaultPatternIfMaskConfigNotFound() {
        // Arrange
        var original = new MasterSystemReference(null, "idee", "5");
        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        config[""] = "";
        var formatter = new DefaultMasterSystemFormatter(config);

        // Act
        string result = formatter.FormatMasterSystemId(original, "");
        const string expected = "idee 5";

        // Assert
        Assert.AreEqual(expected, result);
    }
    [TestMethod, UnitTest]
    public void CanFormatWithMasterSystemBeingNull() {
        // Arrange
        var original = new MasterSystemReference(null, "idee", "5");
        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        config[":idee:-"] = "{0}-{1}";
        var formatter = new DefaultMasterSystemFormatter(config);

        // Act
        string result = formatter.FormatMasterSystemId(original, "-");
        const string expected = "idee-5";

        // Assert
        Assert.AreEqual(expected, result);
    }
}