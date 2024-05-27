using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace idee5.EFCore.Tests;

internal class TestUnitOfWork : EFCoreUnitOfWork<TestDbContext> {
    // ignore repositories etc, just expose the db context
    public TestDbContext DbContext => dbContext;
    public TestUnitOfWork(IDbContextFactory<TestDbContext> dbContextFactory, ILogger? logger) : base(dbContextFactory, logger) {
    }
}