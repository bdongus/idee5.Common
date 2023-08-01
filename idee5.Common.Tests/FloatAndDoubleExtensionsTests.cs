using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Common.Tests {
    [TestClass]
    public class FloatAndDoubleExtensionsTests {
        [TestMethod]
        public void FloatsThatDontMatch() {
            // Arrange
            const float f1 = 0.33333f;
            const float f2 = (float)1 / 3;
            // Act

            // Assert
            Assert.AreNotEqual(f1, f2);
        }

        [TestMethod]
        public void FloatsNearyEqual() {
            // Arrange
            const float f1 = 0.33333f;
            const float f2 = (float)1 / 3;

            // Act
            bool result = f1.NearyEquals(f2, 0.0001f);
            // Assert
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void DoublesThatDontMatch() {
            // Arrange
            const double d1 = 0.33333d;
            const double d2 = (double)1 / 3;
            // Act

            // Assert
            Assert.AreNotEqual(d1, d2);
        }

        [TestMethod]
        public void DoublesNearyEqual() {
            // Arrange
            const double d1 = 0.33333f;
            const double d2 = (double)1 / 3;

            // Act
            bool result = d1.NearyEquals(d2, 0.0001f);
            // Assert
            Assert.IsTrue(result);
        }
    }
}