using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace idee5.Common.Tests {
    [TestClass]
    public class IfExtensionTests {
        private bool _lambdaResult;
        private bool _actionResult;

        public void IfNotNullAction(string item) {
            _actionResult = true;
        }

        [UnitTest, TestMethod]
        public void WontCallActionIfNull() {
            // Arrange
            const string testItem = null;
            _actionResult = false;

            var actionMethod = new Action<string>(IfNotNullAction);
            // Act
            testItem.IfNotNull(actionMethod);
            // Assert
            Assert.IsFalse(_actionResult);
        }

        [UnitTest, TestMethod]
        public void CanCallLambdaIfNotNull() {
            // Arrange
            const string testItem = "test";
            _lambdaResult = false;
            // Act
            testItem.IfNotNull(_ => _lambdaResult = true);
            // Assert
            Assert.IsTrue(_lambdaResult);
        }
    }
}
