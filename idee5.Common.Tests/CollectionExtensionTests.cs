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
            Assert.HasCount(expected: 1, collection);
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
            Assert.HasCount(expected: 2, collection);
        }

        [TestMethod]
        public void ThrowsNullException() {
            // Arrange
            ICollection<string> collection = null;

            // Act
            Assert.ThrowsExactly<ArgumentNullException>(() => collection.AddIfNotContains(null));
        }
    }
}
