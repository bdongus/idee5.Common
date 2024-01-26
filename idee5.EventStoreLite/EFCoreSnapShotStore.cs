using Boxed.Mapping;
using CQRSlite.Snapshotting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace idee5.EventStoreLite;

/// <summary>
/// Storage for <see cref="Snapshot"/>s
/// </summary>
/// <typeparam name="T">Snapshot type, derived from <see cref="Snapshot"/></typeparam>
public class EFCoreSnapShotStore<T> : IDisposable, ISnapshotStore where T : Snapshot{
    private readonly IDbContextFactory<EventStoreDbContext> _dbContextFactory;
    private readonly IMapper<Snapshot, SnapshotEntry> _mapper;
    private EventStoreDbContext _dbContext;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapShotStoreRepository"/> class.
    /// </summary>
    /// <param name="dbContextFactory">Used to create an <see cref="EventStoreDbContext"/> for each transaction</param>
    /// <param name="mapper">The mapper.</param>
    public EFCoreSnapShotStore(IDbContextFactory<EventStoreDbContext> dbContextFactory, IMapper<Snapshot, SnapshotEntry> mapper) {
        ArgumentNullException.ThrowIfNull(dbContextFactory);

        _dbContext = dbContextFactory.CreateDbContext();
        _dbContextFactory = dbContextFactory;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Gets the <see cref="Snapshot"/>
    /// </summary>
    /// <param name="id">The id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns><![CDATA[Task<Snapshot>]]></returns>
    public async Task<Snapshot?> Get(Guid id, CancellationToken cancellationToken = default) {
        SnapshotEntry? se = await _dbContext.SnapshotEntries.SingleOrDefaultAsync(s => s.Id == id, cancellationToken).ConfigureAwait(false);
        T? snapshot = null;
        if (se?.Data != null) {
            snapshot = JsonSerializer.Deserialize<T>(se.Data);
        }
        return snapshot;
    }

    /// <summary>
    /// Save the snapshot
    /// </summary>
    /// <param name="snapshot">The snapshot</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Save(Snapshot snapshot, CancellationToken cancellationToken = default) {
        _dbContext.Add(_mapper.Map(snapshot));
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        // renew the context after saving
        await _dbContext.DisposeAsync().ConfigureAwait(false);
        _dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                _dbContext?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose() {

        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
