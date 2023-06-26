using System.Collections.Generic;

namespace idee5.Common;
/// <summary>
/// The problem details with errors.
/// </summary>
public class ProblemDetailsWithErrors {
    /// <summary>
    /// Gets or Sets the type.
    /// </summary>
    public string Type { get; set; } = null!;
    /// <summary>
    /// Gets or Sets the title.
    /// </summary>
    public string Title { get; set; } = null!;
    /// <summary>
    /// Gets or Sets the status.
    /// </summary>
    public int Status { get; set; }
    /// <summary>
    /// Gets or Sets the trace id.
    /// </summary>
    public string TraceId { get; set; } = null!;
    /// <summary>
    /// The error list.
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }
}
