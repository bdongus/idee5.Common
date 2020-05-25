using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace idee5.Common.Tests {
    [TestClass]
    public class ReflectionUtilsTests
    {
        public static int staticInt ;

        [UnitTest, TestMethod]
        public void CanCreateInstanceFromString()
        {
            // Arrange
            var instance = ReflectionUtils.CreateInstanceFromString("System.DateTime");

            // Act

            // Assert
            Assert.IsInstanceOfType(instance, typeof(DateTime));
        }

        [UnitTest, TestMethod]
        public void CanGetStaticProperty()
        {
            // Arrange
            staticInt = 12;

            // Act
            var result = ReflectionUtils.GetStaticProperty("System.DateTime", "Today");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected: DateTime.Today, actual: result);
        }
    }
}
