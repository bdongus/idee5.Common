using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace idee5.Common.Tests {
    [TestClass]
    public class ObjectExtensionsTests {
        [TestMethod]
        public void CanDetectEnumValueNotInList() {
            // Arrange

            // Act
            bool result = DayOfWeek.Sunday.In(DayOfWeek.Monday, DayOfWeek.Tuesday);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanDetectEnumValueIsInList() {
            // Arrange

            // Act
            bool result = DayOfWeek.Sunday.In(DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Sunday);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnsFalseIfInListIsEmpty() {
            // Arrange

            // Act
            bool result = DayOfWeek.Sunday.In(Array.Empty<DayOfWeek>());

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ReturnsFalseIfInListIsNull() {
            // Arrange

            // Act
            bool result = DayOfWeek.Sunday.In(null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanConvertSameType() {
            // Arrange
            var source = TimeSpan.FromDays(1);

            // Act
            bool result = source.TryConvert(out TimeSpan converted);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(source, converted);
        }

        [TestMethod]
        public void CanConvertStringToInt() {
            // Arrange
            var source = "42";

            // Act
            bool result = source.TryConvert(out int converted);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(42, converted);
        }

        [TestMethod]
        public void ReturnsDefaultIfConversionFailed() {
            // Arrange
            var source = "abc";

            // Act
            bool result = source.TryConvert(out int converted);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(default(int), converted);
        }
    }
}
