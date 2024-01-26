using Boxed.Mapping;
using CQRSlite.Events;
using idee5.Common.Data;
using System.Text.Json;

namespace idee5.EventStoreLite;
/// <summary>
/// Map CQRSlite events to the data store model
/// </summary>
public class EventMapper : IMapper<IEvent, EventEntry> {
    /// <inheritdoc/>
    public void Map(IEvent source, EventEntry destination) {
        Type eventType = source.GetType();
        destination.Data = JsonSerializer.Serialize(source, eventType);
        destination.Id = source.Id;
        // destination.Index is database generated
        destination.EventName = eventType?.FullName ?? eventType?.Name ?? "";
        destination.TimeStamp = source.TimeStamp;
        destination.Version = source.Version;
    }
}
