using Microsoft.EntityFrameworkCore;

namespace idee5.EFCore.Tests;

internal class TestDbContext : DbContext {
    DbSet<TestEntity> TestEntities { get; set; }
    public TestDbContext(DbContextOptions options) : base(options) {
    }
}
