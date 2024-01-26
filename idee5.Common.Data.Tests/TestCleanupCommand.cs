using System;

namespace idee5.Common.Data.Tests;

public class TestCleanupCommand : ICleanupCommand {
    /// <inheritdoc/>
    public DateTime BeforeUTC { get; set; }
}
