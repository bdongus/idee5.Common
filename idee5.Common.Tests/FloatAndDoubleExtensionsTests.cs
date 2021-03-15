using Microsoft.VisualStudio.TestTools.UnitTesting;
using idee5.Common;

namespace idee5.Common.Tests {
    [TestClass]
    public class FloatAndDoubleExtensionsTests {
        [TestMethod]
        public void FloatsThatDontMatch() {
            // Arrange
            const float f1 = 0.15f + 0.15f;
            const float f2 = 0.1f + 0.2f;
            // Act

            // Assert
            Assert.AreEqual(f1, f2);
        }

        [TestMethod]
        public void NearyEqualsTest1() {
            Assert.Fail();
        }
    }
}