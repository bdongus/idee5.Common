using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Common.Tests {
    public class FloatAndDoubleExtensionTests {
        [UnitTest, TestMethod]
        public void CanDetectUnequalFloat() {
            // Arrange
            const float left = 7.4999998883333339F;
            const float right = 7.48F;

            // Act
            bool result = left.NearyEquals(right, 0.001F);

            // Assert
            Assert.IsFalse(result);
        }

        [UnitTest, TestMethod]
        public void CanDetectEqualFloat() {
            // Arrange
            const float left = 7.4999998883333339F;
            const float right = 7.4999998883333339F;

            // Act
            bool result = left.NearyEquals(right, 0.0F);

            // Assert
            Assert.IsTrue(result);
        }
        [UnitTest, TestMethod]
        public void CanDetectNearlyEqualFloat() {
            // Arrange
            const float left = 7.4999998883333339F;
            const float right = 7.5F;

            // Act
            bool result = left.NearyEquals(right, 0.1F);

            // Assert
            Assert.IsTrue(result);
        }

        [UnitTest, TestMethod]
        public void CanDetectUnequalDouble() {
            // Arrange
            const double left = 7.4999998883333339D;
            const double right = 7.48D;

            // Act
            bool result = left.NearyEquals(right, 0.001D);

            // Assert
            Assert.IsFalse(result);
        }

        [UnitTest, TestMethod]
        public void CanDetectEqualDouble() {
            // Arrange
            const double left = 7.4999998883333339D;
            const double right = 7.4999998883333339D;

            // Act
            bool result = left.NearyEquals(right, 0.0D);

            // Assert
            Assert.IsTrue(result);
        }
        [UnitTest, TestMethod]
        public void CanDetectNearlyEqualDouble() {
            // Arrange
            const double left = 7.4999998883333339D;
            const double right = 7.5D;

            // Act
            bool result = left.NearyEquals(right, 0.1D);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
