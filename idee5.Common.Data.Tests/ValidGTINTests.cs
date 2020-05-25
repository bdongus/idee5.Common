using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class ValidGTINTests {
        [TestMethod]
        public void EmptyStringIsValid() {
            // Arrange
            var attribute = new ValidGTINAttribute();

            // Act
            bool result = attribute.IsValid(string.Empty);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NullIsValid() {
            // Arrange
            var attribute = new ValidGTINAttribute();

            // Act
            bool result = attribute.IsValid(null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanValidateGTIN13() {
            // Arrange
            var attribute = new ValidGTINAttribute();

            // Act
            bool result = attribute.IsValid("4025692000004");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDetectInvalidateGTIN13() {
            // Arrange
            var attribute = new ValidGTINAttribute();

            // Act
            bool result = attribute.IsValid("4025692000014");

            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void CanValidateGTIN8() {
            // Arrange
            var attribute = new ValidGTINAttribute();

            // Act
            bool result = attribute.IsValid("76136573");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDetectInvalidateGTIN8() {
            // Arrange
            var attribute = new ValidGTINAttribute();

            // Act
            bool result = attribute.IsValid("76136574");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FailsOnNonString() {
            // Arrange
            var attribute = new ValidGTINAttribute();

            // Act
            bool result = attribute.IsValid(42);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
