using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace idee5.Common.Tests {
    [TestClass]
    public class EnumExtensionTests {
        [TestMethod]
        public void CanConvertToDictionary() {
            // Arrange

            // Act
            Dictionary<int, string> result = EnumUtils<DayOfWeek>.ToDictionary;

            // Assert
            Assert.AreEqual(Enum.GetValues(typeof(DayOfWeek)).Length, result.Count);
            Assert.AreEqual(0, result.First().Key);
        }

        [TestMethod]
        public void ThrowsCountingNonEnum() {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<TypeInitializationException>(() => EnumUtils<DateTime>.Count);
            Assert.ThrowsException<TypeInitializationException>(() => EnumUtils<byte>.Count);
        }

        [TestMethod]
        public void CanCountEnumMembers() {
            // Arrange

            // Act
            var result = EnumUtils<DayOfWeek>.Count;

            // Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void CanGenerateMultipleDictionaries() {
            // Arrange

            // Act
            Dictionary<int, string> result = EnumUtils<DayOfWeek>.ToDictionary;
            Dictionary<int, string> result2 = EnumUtils<DateTimeKind>.ToDictionary;

            // Assert
            Assert.AreEqual(Enum.GetValues(typeof(DayOfWeek)).Length, result.Count);
            Assert.AreEqual(Enum.GetValues(typeof(DateTimeKind)).Length, result2.Count);
            Assert.AreEqual(0, result.First().Key);
        }

        [TestMethod]
        public void CanToString() {
            // Arrange

            // Act
            var result = EnumUtils<DayOfWeek>.ToString(DayOfWeek.Monday);

            // Assert
            Assert.AreEqual("Monday", result);
        }
    }
}
