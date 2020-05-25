using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace idee5.Common.Net.Tests {
    [TestClass]
    public class SntpTimeProviderTests
    {
        [TestMethod]
        public void CanGetTime()
        {
            // Arrange
            var timeProvider = new SntpTimeProvider();

            // Act
            DateTime result = timeProvider.UtcNow;

            // Assert
            Assert.IsTrue(result != null);
            Assert.AreNotEqual(default, result);
            Assert.AreEqual(DateTimeKind.Utc, result.Kind);
        }

        [TestMethod]
        public async Task CanGetTimeAsync()
        {
            // Arrange
            var timeProvider = new AsyncSntpTimeProvider();

            // wait for initalization
            await timeProvider.Initialization;

            // Act
            DateTime result = timeProvider.UtcNow;

            // Assert
            Assert.IsTrue(result != null);
            Assert.AreNotEqual(default, result);
            Assert.AreEqual(DateTimeKind.Utc, result.Kind);
        }
    }
}