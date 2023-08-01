using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class ConnectionStringProviderTests {
        [TestMethod]
        public void CanCreateValidDB3ConnectionString() {
            // Arrange
            var provider = new SimpleDB3ConnectionStringProvider();
            const string connectionId = "idee5";
            var path = AppDomain.CurrentDomain.GetDataDirectory();

            // Act
            var result = provider.GetConnectionString(connectionId);

            // Assert
            Assert.AreEqual($"Data Source={Path.Combine(path, connectionId)}.db3", result);
        }
    }
}
