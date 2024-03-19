using System;
using System.Threading.Tasks;
using System.Threading;

namespace idee5.Common.Data;
/// <summary>
/// The don't clean up command
/// </summary>
public class NoCleanupCommand : ICleanupCommand {
    /// <inheritdoc/>
    public DateTime BeforeUTC { get; set; }
}
