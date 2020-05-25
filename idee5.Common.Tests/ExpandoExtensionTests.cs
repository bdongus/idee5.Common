using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;

namespace idee5.Common.Tests {
    [TestClass]
    public class ExpandoExtensionTests
    {
        [UnitTest, TestMethod]
        public void CanAddPropertyByName()
        {
            // Arrange
            var sut = new ExpandoObject();

            // Act
            sut.AddProperty("idee5", "Swiss");

            // Assert
            Assert.AreEqual("Swiss", ((dynamic)sut).idee5);
        }

        [UnitTest, TestMethod]
        public void CanGetPropertyByName()
        {
            // Arrange
            var sut = new ExpandoObject();
            sut.AddProperty("idee5", "Swiss");

            // Act
            var result = sut.GetPropertyByName("idee5").ToString();

            // Assert
            Assert.AreEqual("Swiss", result);
        }
    }
}
