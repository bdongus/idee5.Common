using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class ValidationReporterTests {
        [TestMethod]
        public void CanReportErrorMessageToConsole() {
            // Arrange
            var reporter = new ConsoleValidationReporter();
            var validationResult = new ValidationResult("This is an error");
            string result;
            TextWriter oldConsoleOut = Console.Out;
            // Act
            using (var sw = new StringWriter()) {
                Console.SetOut(sw);
                reporter.Report(validationResult);
                result = sw.ToString();
            }
            Console.SetOut(oldConsoleOut);
            Console.Write(result);
            // Assert
            Assert.AreEqual(validationResult.ErrorMessage.Length + 23 + 2 * Environment.NewLine.Length, result.Length);
        }
    }
}
