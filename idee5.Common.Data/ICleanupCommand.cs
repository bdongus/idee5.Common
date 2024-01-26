using System;

namespace idee5.Common.Data;
/// <summary>
/// The cleanup base command parameter
/// </summary>
public interface ICleanupCommand {
    /// <summary>
    /// UTC-Timestamp. All entities being last modified before will be removed.
    /// </summary>
    public DateTime BeforeUTC { get; set; }
}
