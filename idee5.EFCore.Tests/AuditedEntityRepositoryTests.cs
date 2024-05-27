using idee5.Common;

using Microsoft.Data.Sqlite;

namespace idee5.EFCore.Tests;
[TestClass]
public class TestEntityRepositoryTests : IDisposable {
    private readonly SqliteConnection _connection;
    private readonly TestDbContextFactory _factory;
    private readonly DefaultTimeProvider _timeProvider = new();
    private readonly DefaultCurrentUserIdProvider _userProvider = new();
    private bool _disposedValue;
    public TestEntityRepositoryTests() {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _factory = new TestDbContextFactory(_connection);
    }
    [TestMethod]
    public void CanAuditOnCreate() {
        // Arrange
        var dbContext = _factory.CreateDbContext();
        var repository = new AuditedTestRepository(dbContext, _timeProvider, _userProvider);
        var entity = new TestEntity();

        // Act
        repository.Add(entity);

        // Assert
        Assert.IsTrue(entity.CreatedBy.HasValue());
    }

    [TestMethod]
    public void CanAuditOnUpdate() {
        // Arrange
        var dbContext = _factory.CreateDbContext();
        var repository = new AuditedTestRepository(dbContext, _timeProvider, _userProvider);
        var entity = new TestEntity() { Id = 1 };
        repository.Add(entity);

        // Act
        repository.Update(entity);

        // Assert
        Assert.IsTrue(entity.ModifiedBy.HasValue());
    }

    [TestMethod]
    public async Task CanAuditCreateOnUpdateOrAddAsync() {
        // Arrange
        var dbContext = _factory.CreateDbContext();
        var repository = new AuditedTestRepository(dbContext, _timeProvider, _userProvider);
        var entity = new TestEntity() { Id = 2};

        // Act
        await repository.UpdateOrAddAsync(entity).ConfigureAwait(false);

        // Assert
        Assert.IsTrue(entity.CreatedBy.HasValue());
    }

    [TestMethod]
    public async Task CanAuditUpdateOnUpdateOrAddAsync() {
        // Arrange
        var dbContext = _factory.CreateDbContext();
        var repository = new AuditedTestRepository(dbContext, _timeProvider, _userProvider);
        var entity = new TestEntity() { Id = 3 };
        repository.Add(entity);
        await dbContext.SaveChangesAsync();
        // Act
        entity.SomeString = "idee5";
        await repository.UpdateOrAddAsync(entity).ConfigureAwait(false);

        // Assert
        Assert.IsTrue(entity.ModifiedBy.HasValue());
    }
    [TestMethod]
    public async Task DoesntChangedModifiedByOnUpdateOfUncomittedCreateAsync() {
        // Arrange
        var dbContext = _factory.CreateDbContext();
        var repository = new AuditedTestRepository(dbContext, _timeProvider, _userProvider);
        var entity = new TestEntity() { Id = 4 };
        repository.Add(entity);

        // Act
        entity.SomeString = "idee5";
        await repository.UpdateOrAddAsync(entity).ConfigureAwait(false);

        // Assert
        Assert.IsFalse(entity.ModifiedBy.HasValue());
    }
    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                _connection.Close();
            }

            _disposedValue = true;
        }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
