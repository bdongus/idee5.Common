using Microsoft.VisualStudio.TestTools.UnitTesting;
using idee5.Common.Data;
using System;
using System.Collections.Generic;
using System.Text;
using MELT;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;

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