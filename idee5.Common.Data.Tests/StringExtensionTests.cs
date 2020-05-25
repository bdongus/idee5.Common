using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod, UnitTest]
        public void UsesBaseDirectoryIfDataDirectoryNotSet()
        {
            // Arrange
            AppDomain.CurrentDomain.SetData("DataDirectory", null);
            const string testString = "|DataDirectory|idee5.txt";
            string expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "idee5.txt");

            // Act
            string result = testString.ReplaceDataDirectory();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod, UnitTest]
        public void CanReplaceDataDirectoryFromAppDomain()
        {
            // Arrange
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(Environment.CurrentDirectory, "App_Data"));
            const string testString = "|DataDirectory|idee5.txt";
            string expected = Environment.CurrentDirectory + "\\App_Data\\idee5.txt";

            // Act
            string result = testString.ReplaceDataDirectory();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod, UnitTest]
        public void CanHandleMissingPlaceholder ()
        {
            // Arrange
            const string testString = "idee5.txt";
            const string expected = "idee5.txt";

            // Act
            string result = testString.ReplaceDataDirectory();

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}