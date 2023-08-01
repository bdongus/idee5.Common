using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace idee5.Common.Tests {
    [TestClass]
    public class TimeSpanTests {
        [UnitTest, TestMethod]
        public void CanRoundUp() {
            // Arrange
            var start = TimeSpan.FromMinutes(value: 23);
            // Act
            TimeSpan actual = start.RoundToNearest(TimeSpan.FromMinutes(value: 15));
            // Assert
            Assert.AreEqual(expected: TimeSpan.FromMinutes(value: 30), actual: actual);
        }

        [UnitTest, TestMethod]
        public void CanRoundDown() {
            // Arrange
            var start = TimeSpan.FromMinutes(value: 36);
            // Act
            TimeSpan actual = start.RoundToNearest(TimeSpan.FromMinutes(value: 15));
            // Assert
            Assert.AreEqual(expected: TimeSpan.FromMinutes(value: 30), actual: actual);
        }
        // the multiplication and division tests are obsolete for .net core, they are part of the framework.

        [UnitTest, TestMethod]
        public void CanCreateMinutesFromInt() {
            // Arrange
            const int testValue = 2;

            // Act
            TimeSpan actual = testValue.Minutes();

            // Assert
            Assert.AreEqual(TimeSpan.FromMinutes(testValue), actual);
        }

        [UnitTest, TestMethod]
        public void CanCreateSecondsFromInt() {
            // Arrange
            const int testValue = 2;

            // Act
            TimeSpan actual = testValue.Seconds();

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(testValue), actual);
        }
    }
}