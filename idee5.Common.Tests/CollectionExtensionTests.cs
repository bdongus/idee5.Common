using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace idee5.Common.Tests {
    [TestClass]
    public class CollectionExtensionTests {
        [UnitTest, TestMethod]
        public void CantAddIfContains() {
            // Arrange
            var collection = new Collection<String> {
                "blabla"
            };
            // Act
            collection.AddIfNotContains("blabla");

            // Assert
            Assert.AreEqual(expected: 1, actual: collection.Count);
        }

        [UnitTest, TestMethod]
        public void CanAddIfNotContains() {
            // Arrange
            var collection = new Collection<String> {
                "blabla"
            };
            // Act
            collection.AddIfNotContains("blablubb");

            // Assert
            Assert.AreEqual(expected: 2, actual: collection.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsNullException() {
            // Arrange
            ICollection<String> collection = null;

            // Act
            collection.AddIfNotContains(null);

            // Assert - exception expected
        }
    }
}
