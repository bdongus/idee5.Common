using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;

namespace idee5.Common.Data.Tests {
    public class TestValuesAttribute : AllowedValuesAttribute {
        protected override string GetInvalidValueMessage(object invalidValue, object[] validValues) {
            var valid = String.Join(", ", validValues);
            return $"{invalidValue} is not a valid or allowed. Options are: [{valid}]";
        }

        protected override object[] GetValues(ValidationContext validationContext) => new object[] { 1, 2, 3 };
    }
    [TestClass]
    public class AllowedValuesTests {
        [TestMethod]
        public void CanValidateInvalidIntValue() {
            // Arrange
            var attr = new TestValuesAttribute();

            // Act
            bool result = attr.IsValid(42);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanValidateValidIntValue() {
            // Arrange
            var attr = new TestValuesAttribute();

            // Act
            bool result = attr.IsValid(2);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
