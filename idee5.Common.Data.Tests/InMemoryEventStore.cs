using CQRSlite.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    public class InMemoryEventStore : AbstractEventStore<IEvent>, IEventStore {
        #region Private Fields

        private readonly Dictionary<Guid, List<EventEntry>> _inMemoryDb = new();
        private readonly IEventPublisher _publisher;

        #endregion Private Fields

        #region Public Constructors

        public InMemoryEventStore(IEventPublisher publisher) {
            _publisher = publisher;
        }

        #endregion Public Constructors

        #region Public Methods

        public Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default) {
            List<IEvent> result = new List<IEvent>();
            if (aggregateId != default) {
                _inMemoryDb.TryGetValue(aggregateId, out List<EventEntry> eventEntries);
                foreach (var item in eventEntries ?? Enumerable.Empty<EventEntry>()) {
                    result.Add(CreateEvent(item));
                }
            }

            // Task.FromResult because our implementation is synchronous.
            return Task.FromResult(result?.Where(x => x.Version > fromVersion) ?? new List<IEvent>());
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default) {
            foreach (IEvent source in events) {
                _inMemoryDb.TryGetValue(source.Id, out List<EventEntry> list);
                if (list == null) {
                    list = new List<EventEntry>();
                    _inMemoryDb.Add(source.Id, list);
                }
                Type eventType = source.GetType();
                var destination = new EventEntry {
                    Data = JsonSerializer.Serialize(source, eventType),
                    Id = source.Id,
                    // destination.Index is database generated
                    EventName = eventType.AssemblyQualifiedName,
                    TimeStamp = source.TimeStamp,
                    Version = source.Version
                };

                list.Add(destination);
                await _publisher.Publish(source, cancellationToken).ConfigureAwait(false);
            }
        }

        protected override void AdditionalMappings(ref IEvent ev, Dictionary<string, JsonElement> dict) {
            ev.TimeStamp = DateTimeOffset.Parse(dict[nameof(IEvent.TimeStamp).CamelToPascalCase()].ToString());
            ev.Version = int.Parse(dict[nameof(IEvent.Version).CamelToPascalCase()].ToString());
        }

        #endregion Public Methods
    }
}
