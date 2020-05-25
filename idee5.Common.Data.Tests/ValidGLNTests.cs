using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class ValidGLNTests {
        [TestMethod]
        public void EmptyStringIsValid() {
            // Arrange
            var attribute = new ValidGLNAttribute();

            // Act
            bool result = attribute.IsValid(string.Empty);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NullIsValid() {
            // Arrange
            var attribute = new ValidGLNAttribute();

            // Act
            bool result = attribute.IsValid(null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanValidateGLN() {
            // Arrange
            var attribute = new ValidGLNAttribute();

            // Act
            bool result = attribute.IsValid("4025692000004");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDetectInvalidateGLN() {
            // Arrange
            var attribute = new ValidGLNAttribute();

            // Act
            bool result = attribute.IsValid("4025692000014");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FailsOnNonString() {
            // Arrange
            var attribute = new ValidGLNAttribute();

            // Act
            bool result = attribute.IsValid(42);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
