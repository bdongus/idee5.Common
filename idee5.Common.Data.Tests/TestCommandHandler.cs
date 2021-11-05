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

    internal class TestCommandHandler : ICommandHandlerAsync<TestCommand> {
        public Task HandleAsync(TestCommand command, CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }
    }
}