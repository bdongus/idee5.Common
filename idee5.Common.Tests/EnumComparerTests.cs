using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace idee5.Common.Tests {
    [TestClass]
    public class EnumComparerTests {

        protected IEqualityComparer<TEnum> getEnumComparer<TEnum>() where TEnum : struct, IComparable, IConvertible, IFormattable {
            return EnumComparer.For<TEnum>();
        }

        [TestMethod]
        public void CanDetectDifferingValues() {
            // Arrange
            IEqualityComparer<DayOfWeek> enumComparer = getEnumComparer<DayOfWeek>();
            const DayOfWeek left = DayOfWeek.Monday;
            const DayOfWeek right = DayOfWeek.Friday;

            // Act
            var result = enumComparer.Equals(left, right);

            // Assert
            Assert.AreEqual(left == right, result);
        }

        [TestMethod]
        public void CanCompareMatchingValues() {
            // Arrange
            IEqualityComparer<DayOfWeek> enumComparer = getEnumComparer<DayOfWeek>();
            const DayOfWeek left = DayOfWeek.Monday;
            const DayOfWeek right = DayOfWeek.Monday;

            // Act
            var result = enumComparer.Equals(left, right);

            // Assert
            Assert.AreEqual(left == right, result);
        }

        [TestMethod]
        public void GetHashCode_should_return_hashCode_of_literal() {
            // Arrange
            IEqualityComparer<DayOfWeek> enumComparer = getEnumComparer<DayOfWeek>();

            // Act
            var hashCode = enumComparer.GetHashCode(DayOfWeek.Monday);

            // Assert
            Assert.AreEqual(DayOfWeek.Monday.GetHashCode(), hashCode);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ThrowsOnIntAsEnum() {
            // Arrange

            // Act
            _ = getEnumComparer<int>();
            // Assert         
        }
    }
}
