using MELT;
using Microsoft.Extensions.Logging;

namespace idee5.Common.Data.Tests {
    internal class TestLoggerCreator : ILoggerFactory {
        private readonly ITestLoggerFactory _loggerFactory;

        public TestLoggerCreator(ITestLoggerFactory loggerFactory) {
            _loggerFactory = loggerFactory;
        }
        public void AddProvider(ILoggerProvider provider) {
        }

        public ILogger CreateLogger(string categoryName) {
            return _loggerFactory.CreateLogger(categoryName);
        }

        public void Dispose() {
        }
    }
}