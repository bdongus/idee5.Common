using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class AppDomainExtensionTests
    {
        [TestMethod, UnitTest]
        public void CanGetDataDirectory()
        {
            // Arrange
            AppDomain.CurrentDomain.SetData("DataDirectory", "App_Data");
            const string expected = "App_Data";

            // Act
            string result = AppDomain.CurrentDomain.GetDataDirectory();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod, UnitTest]
        public void CanGetBaseDirectory()
        {
            // Arrange
            AppDomain.CurrentDomain.SetData("DataDirectory", null);

            string expected = AppDomain.CurrentDomain.BaseDirectory;

            // Act
            string result = AppDomain.CurrentDomain.GetDataDirectory();

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}