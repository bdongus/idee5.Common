using MELT;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        [TestMethod]
        public void CanReportToLog() {
            // Arrange
            ITestLoggerFactory loggerFactory = TestLoggerFactory.Create();
            ILogger logger = loggerFactory.CreateLogger("test");
            var reporter = new LogValidationReporter(logger);
            var validationResult = new ValidationResult("");

            // Act
            reporter.Report(validationResult);

            // Assert
            Assert.AreEqual("Member names : ", loggerFactory.Sink.LogEntries.First().Message);
        }

        [TestMethod]
        public void CanReportMissingResultToLog() {
            // Arrange
            ITestLoggerFactory loggerFactory = TestLoggerFactory.Create();
            ILogger logger = loggerFactory.CreateLogger("test");
            var reporter = new LogValidationReporter(logger);

            // Act
            reporter.Report(null);

            // Assert
            Assert.AreEqual("Validation result is NULL.", loggerFactory.Sink.LogEntries.First().Message);
        }

        [TestMethod]
        public async Task CanReportToLogAsync() {
            // Arrange
            ITestLoggerFactory loggerFactory = TestLoggerFactory.Create();
            ILogger logger = loggerFactory.CreateLogger("test");
            var reporter = new LogValidationReporter(logger);
            var validationResult = new ValidationResult("");

            // Act
            await reporter.ReportAsync(validationResult).ConfigureAwait(false);

            // Assert
            Assert.AreEqual("Member names : ", loggerFactory.Sink.LogEntries.First().Message);
        }

        [TestMethod]
        public async Task CanReportMissingResultToLogAsync() {
            // Arrange
            ITestLoggerFactory loggerFactory = TestLoggerFactory.Create();
            ILogger logger = loggerFactory.CreateLogger("test");
            var reporter = new LogValidationReporter(logger);

            // Act
            await reporter.ReportAsync(null).ConfigureAwait(false);

            // Assert
            Assert.AreEqual("Validation result is NULL.", loggerFactory.Sink.LogEntries.First().Message);
        }
    }
}
