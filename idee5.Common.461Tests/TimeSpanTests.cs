using idee5.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace idee5.Common_461Tests {
    [TestClass]
    public class TimeSpanTests
    {
        [UnitTest, TestMethod]
        public void CanRoundUp()
        {
            // Arrange
            var start = TimeSpan.FromMinutes(value: 23);
            // Act
            TimeSpan actual = start.RoundToNearest(TimeSpan.FromMinutes(value: 15));
            // Assert
            Assert.AreEqual(expected: TimeSpan.FromMinutes(value: 30), actual: actual);
        }

        [UnitTest, TestMethod]
        public void CanRoundDown()
        {
            // Arrange
            var start = TimeSpan.FromMinutes(value: 36);
            // Act
            TimeSpan actual = start.RoundToNearest(TimeSpan.FromMinutes(value: 15));
            // Assert
            Assert.AreEqual(expected: TimeSpan.FromMinutes(value: 30), actual: actual);
        }

        [UnitTest, TestMethod]
        public void CanMultiplyTimeSpanByInt()
        {
            // Arrange
            var span = TimeSpan.FromMinutes(24);
            const int multiplier = 3;

            // Act
            TimeSpan result = span.Multiply(multiplier);

            // Assert
            Assert.AreEqual(TimeSpan.FromMinutes(72), result);
        }

        [UnitTest, TestMethod]
        public void CanMultiplyTimespanByFloat()
        {
            // Arrange
            var span = TimeSpan.FromMinutes(24);
            const float multiplier = 3.5f;

            // Act
            TimeSpan result = span.Multiply(multiplier);

            // Assert
            Assert.AreEqual(TimeSpan.FromMinutes(84), result);
        }

        [UnitTest, TestMethod]
        public void CanMultiplyTimespanByDouble()
        {
            // Arrange
            var span = TimeSpan.FromMinutes(24);
            const double multiplier = 3.5d;

            // Act
            TimeSpan result = span.Multiply(multiplier);

            // Assert
            Assert.AreEqual(TimeSpan.FromMinutes(84), result);
        }

        [UnitTest, TestMethod]
        public void CanMultiplyTimespanByLong()
        {
            // Arrange
            var span = TimeSpan.FromMinutes(24);
            const long multiplier = 3L;

            // Act
            TimeSpan result = span.Multiply(multiplier);

            // Assert
            Assert.AreEqual(TimeSpan.FromMinutes(72), result);
        }

        [UnitTest, TestMethod]
        public void CanDivideTimeSpanByInt()
        {
            // Arrange
            var span = TimeSpan.FromMinutes(24);
            const int divisor = 3;

            // Act
            TimeSpan result = span.Divide(divisor);

            // Assert
            Assert.AreEqual(TimeSpan.FromMinutes(8), result);
        }

        [UnitTest, TestMethod]
        public void CanDivideTimespanByFloat()
        {
            // Arrange
            var span = TimeSpan.FromMinutes(24);
            const float divisor = 3.2f;

            // Act
            TimeSpan result = span.Divide(divisor);

            // Assert
            Assert.IsTrue(result.TotalMinutes.NearyEquals(7.5F, 0.1F));
        }

        [UnitTest, TestMethod]
        public void CanDivideTimespanByDouble()
        {
            // Arrange
            var span = TimeSpan.FromMinutes(24);
            const double divisor = 3.2d;

            // Act
            TimeSpan result = span.Divide(divisor);

            // Assert
            Assert.AreEqual(TimeSpan.FromMinutes(7.5), result);
        }

        [UnitTest, TestMethod]
        public void CanDivideTimespanByLong()
        {
            // Arrange
            var span = TimeSpan.FromMinutes(24);
            const long divisor = 3L;

            // Act
            TimeSpan result = span.Divide(divisor);

            // Assert
            Assert.AreEqual(TimeSpan.FromMinutes(8), result);
        }
    }
}