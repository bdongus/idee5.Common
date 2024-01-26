using idee5.EventStoreLite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
namespace idee5.EventstorLiteTests;

public class TestDbContextFactory : IDbContextFactory<EventStoreDbContext> {
    private DbContextOptions<EventStoreDbContext> _options;

    public TestDbContextFactory(SqliteConnection connection, ITestOutputHelper output) {
        _options = new DbContextOptionsBuilder<EventStoreDbContext>().UseSqlite(connection)
            .LogTo(output.WriteLine)
            .EnableSensitiveDataLogging().Options;
        var dbContext = new EventStoreDbContext(_options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.Dispose();
    }

    public EventStoreDbContext CreateDbContext() => new EventStoreDbContext(_options);
}
