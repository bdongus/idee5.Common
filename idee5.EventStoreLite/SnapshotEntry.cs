using CQRSlite.Snapshotting;
using idee5.Common.Data;
using System.ComponentModel.DataAnnotations;

namespace idee5.EventStoreLite;

/// <summary>
/// The snapshot entry.
/// </summary>
public class SnapshotEntry : Snapshot, IGuidEntity {
    /// <summary>
    /// Clustering index.
    /// </summary>
    [Key]
    public int Index { get; set; }

    /// <summary>
    /// Serialized snapshot state data.
    /// </summary>
    [MaxLength]
    public string Data { get; set; } = "";

    /// <summary>
    /// Name of the snapshot type.
    /// </summary>
    [Required]
    public string SnapshotTypeName { get; set; } = "";
}
