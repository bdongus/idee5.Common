using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace idee5.Common.Tests {
    [TestClass]
    public class TypeExtensionTests {
        public static class AppMessages {
            public const string DateOutOfRange = "DateOOR";
        }

        public int TestIntProperty { get; set; }

        [UnitTest, TestMethod]
        public void CanGetConstantValue() {
            // Arrange
            // Act
            IEnumerable<string> constValues = typeof(AppMessages).GetAllPublicConstantValues<string>();
            // Assert
            Assert.AreEqual(expected: "DateOOR", actual: constValues.First());
        }

        [UnitTest, TestMethod]
        public void CanDetectClrType() {
            // Arrange
            var propertyType = TestIntProperty.GetType();
            // Act

            bool isClrType = propertyType.IsClrType();

            // Assert
            Assert.IsTrue(isClrType);
        }

        [TestMethod]
        public void CanDetectNonClrType() {
            // Arrange
            var propertyType = typeof(AppMessages);
            // Act

            bool isClrType = propertyType.IsClrType();

            // Assert
            Assert.IsFalse(isClrType);
        }
    }
}