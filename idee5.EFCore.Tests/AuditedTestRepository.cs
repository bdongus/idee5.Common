using idee5.Common;

namespace idee5.EFCore.Tests;

internal class AuditedTestRepository : EFCoreAuditingRepository<TestEntity, int, TestDbContext> {

    public AuditedTestRepository(TestDbContext dbContext, ITimeProvider timeProvider, ICurrentUserIdProvider currentUserProvider) : base(dbContext, timeProvider, currentUserProvider) {
    }
}
