using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Snapshotting;
using idee5.EventStoreLite;
using Microsoft.Data.Sqlite;
using Xunit.Abstractions;

namespace idee5.EventstorLiteTests;

/// <summary>
/// The SQLite event and snapshot store tests.
/// </summary>
public partial class SQLiteInMemTests : IDisposable {
    readonly TestDbContextFactory _factory;
    private bool _disposedValue;
    readonly SqliteConnection _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLiteInMemTests"/> class.
    /// </summary>
    public SQLiteInMemTests(ITestOutputHelper output) {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _factory = new TestDbContextFactory(_connection, output);
    }

    /// <summary>
    /// Fires the event after save
    /// </summary>
    [Fact]
    public async Task EventStoreFiresEventAfterSave() {
        // Arrange
        var eventPublisher = new TestEventPublisher();
        var store = new EFCoreEventStore(eventPublisher, new EventMapper(), _factory);
        var ev = new TestAggregateCreated(new Guid(), "gargl", "fartzen");
        var eventList = new List<IEvent>() { ev };

        // Act
        await store.Save(eventList);

        // Assert
        Assert.Equal(1, eventPublisher.Published);
    }

    /// <summary>
    /// Can get events
    /// </summary>
    [Fact]
    public async Task CanGetEventsAsync() {
        // Arrange
        var eventPublisher = new TestEventPublisher();
        var store = new EFCoreEventStore(eventPublisher, new EventMapper(), _factory);
        var repository = new Repository(store);
        var session = new Session(repository);
        var aggregateId = Guid.NewGuid();
        var testAggregate = new TestAggregate(aggregateId, "idee5", null);

        // Act
        await session.Add(testAggregate);
        await session.Commit();
        IEnumerable<IEvent> result = await store.Get(aggregateId, 0);

        // Assert
        Assert.Single(result);
        Assert.Equal(nameof(TestAggregateCreated), result.First().GetType().Name);
    }

    /// <summary>
    /// Can handle snapshots
    /// </summary>
    [Fact]
    public async Task CanHandleSnapshot() {
        // Arrange
        var guid = Guid.NewGuid();
        var eventPublisher = new TestEventPublisher();
        var store = new EFCoreEventStore(eventPublisher, new EventMapper(), _factory);
        var snapshotStore = new EFCoreSnapShotStore<TestSnapshot>(_factory, new SnapshotMapper());
        var snapshotStrategy = new DefaultSnapshotStrategy();
        var repository = new SnapshotRepository(snapshotStore, snapshotStrategy, new Repository(store), store);
        var session = new Session(repository);
        var aggregate = new TestAggregate(guid, "MQT", null);
        // Act
        for (int i = 0; i < 123; i++) {
            aggregate.Describe(Path.GetRandomFileName());
        }
        aggregate.Describe("idee5");
        await session.Add(aggregate);
        await session.Commit();
        TestAggregate result = await session.Get<TestAggregate>(aggregate.Id);
        // Assert
        Assert.Equal(aggregate.Version, result.Version);
        Assert.Equal("idee5", aggregate.Description);
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                _connection.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
