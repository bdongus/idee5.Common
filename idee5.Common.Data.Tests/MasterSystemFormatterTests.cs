using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class MasterSystemFormatterTests {
        internal class MasterSystemEntity : IMasterSystemReference {
            public string MasterSystemHierarchy { get; set; }
            public string MasterSystemId { get; set; }

            public string MasterSystem { get; set; }
        }

        [TestMethod, UnitTest]
        public void CanFormatDefault() {
            // Arrange
            var original = new MasterSystemEntity() { MasterSystemHierarchy = "idee", MasterSystemId = "5" };
            var formatter = new DefaultMasterSystemFormatter();

            // Act
            string result = formatter.FormatMasterSystemId(original);
            const string expected = "idee 5";

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}