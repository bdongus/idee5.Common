using Boxed.Mapping;
using CQRSlite.Events;
using idee5.Common;
using idee5.Common.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace idee5.EventStoreLite;
/// <summary>
/// The event store.
/// </summary>
public class EFCoreEventStore : AbstractEventStore<IEvent>, IEventStore, IDisposable {
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper<IEvent, EventEntry> _mapper;
    private readonly IDbContextFactory<EventStoreDbContext> _dbContextFactory;
    private bool _disposedValue;
    private EventStoreDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreEventStore"/> class.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="dbContextFactory">Used to create an <see cref="EventStoreDbContext"/> for each transaction</param>
    public EFCoreEventStore(IEventPublisher eventPublisher, IMapper<IEvent, EventEntry> mapper, IDbContextFactory<EventStoreDbContext> dbContextFactory) {
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _dbContext = dbContextFactory.CreateDbContext();
    }

    /// <summary>
    /// Gets the list of events.
    /// </summary>
    /// <param name="aggregateId">The aggregate id.</param>
    /// <param name="fromVersion">The from version. -1 = from the beginning.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<IEnumerable<IEvent>>]]></returns>
    public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default) {
        var result = new List<IEvent>();
        if (aggregateId != default) {
            List<EventEntry> eventEntries = await _dbContext.EventEntries
                .Where(e => e.Id == aggregateId && e.Version > fromVersion).ToListAsync(cancellationToken).ConfigureAwait(false);
            foreach (EventEntry item in eventEntries.WhereNotNull()) {
                result.Add(CreateEvent(item)!);
            }
        }
        return result;
    }

    /// <summary>
    /// Save a list of events.
    /// </summary>
    /// <param name="events">The events.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default) {
        _dbContext.AddRange(_mapper.MapArray(events));
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        // renew the context after saving
        await _dbContext.DisposeAsync().ConfigureAwait(false);
        _dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        foreach (IEvent e in events) {
            await _eventPublisher.Publish(e, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Map the version and timestamp.
    /// Called from <see cref="AbstractEventStore{TEvent}.CreateEvent(EventEntry)"/>
    /// </summary>
    /// <param name="ev">Reference to the event to process</param>
    /// <param name="dict">Property dictionary</param>
    protected override void AdditionalMappings(ref IEvent? ev, Dictionary<string, JsonElement> dict) {
        if (ev != null) {
            ArgumentNullException.ThrowIfNull(dict);
            ev.TimeStamp = DateTimeOffset.Parse(dict[nameof(IEvent.TimeStamp).CamelToPascalCase()].ToString());
            ev.Version = int.Parse(dict[nameof(IEvent.Version).CamelToPascalCase()].ToString());
        }
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                _dbContext.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}