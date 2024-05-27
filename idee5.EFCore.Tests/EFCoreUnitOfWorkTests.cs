using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using MELT;

namespace idee5.EFCore.Tests;

[TestClass()]
public class EFCoreUnitOfWorkTests : IDisposable {
    private readonly TestDbContextFactory _factory;
    private readonly SqliteConnection _connection;
    private bool _disposedValue;

    public EFCoreUnitOfWorkTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _factory = new TestDbContextFactory(_connection);

    }

    [TestMethod()]
    public void ThrowsOnMissingContextFactory() {
        // Arrange


        // Act
        Func<object?> action = () => new TestUnitOfWork(null, null);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }

    [TestMethod()]
    public async Task SaveChangesAsyncTest() {
        // Arrange
        var lf = TestLoggerFactory.Create();
        var logger = lf.CreateLogger("lsmf");
        var uow = new TestUnitOfWork(_factory, logger);
        var entity = new TestEntity() { Id = 1 };
        uow.DbContext.Add(entity);
        string creator = entity.CreatedBy;

        await uow.SaveChangesAsync();

        // Act
        // simulate a change from somewhere else
        await uow.DbContext.Database.ExecuteSqlRawAsync("UPDATE TestEntities set ModifiedBy = 'idee5', Ts=randomblob(8) where Id=1");

        entity.ModifiedBy = "lsmf";
        await uow.SaveChangesAsync();

        // Assert
        Assert.AreEqual(1, lf.Sink.LogEntries.Count());
        Assert.AreEqual("ConcurrencyConflict", lf.Sink.LogEntries.First().EventId.Name);
        Assert.AreEqual("lsmf", entity.ModifiedBy);
        Assert.AreEqual(creator, entity.CreatedBy);

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
        // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}