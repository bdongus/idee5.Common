using Boxed.Mapping;
using CQRSlite.Snapshotting;
using System.Text.Json;

namespace idee5.EventStoreLite;

/// <summary>
/// The snapshot mapper.
/// </summary>
public class SnapshotMapper : IMapper<Snapshot, SnapshotEntry> {
    /// <inheritdoc/>
    public void Map(Snapshot source, SnapshotEntry destination) {
        Type snapshotType = source.GetType();
        destination.SnapshotTypeName = snapshotType.IsGenericType ? snapshotType.GenericTypeArguments[0].Name : snapshotType.Name;

        destination.Data = JsonSerializer.Serialize(source, snapshotType);
        destination.Id = source.Id;
        // destination.Index = will be assigned by the database
        destination.Version = source.Version;
    }
}
