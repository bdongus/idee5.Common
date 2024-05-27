using Dorssel.EntityFrameworkCore;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace idee5.EFCore.Tests;
internal class TestDbContextFactory : IDbContextFactory<TestDbContext> {
    private DbContextOptions<TestDbContext> _options;

    public TestDbContextFactory(SqliteConnection connection) {
        _options = new DbContextOptionsBuilder<TestDbContext>().UseSqlite(connection).UseSqliteTimestamp()
            .LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging().Options;
        var dbContext = new TestDbContext(_options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.Dispose();
    }

    public TestDbContext CreateDbContext() => new TestDbContext(_options);
}