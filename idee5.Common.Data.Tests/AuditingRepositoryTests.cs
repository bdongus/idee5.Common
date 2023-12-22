using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests;
[TestClass]
public class AuditingRepositoryTests {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
    private readonly AmbientTimeProvider _timeProvider = new();
    private readonly AmbientUserProvider _userProvider = new();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
    internal class AuditedEntity : IAuditedEntity, IEntity {
        public DateTime DateCreatedUTC { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateModifiedUTC { get; set; }
        public string ModifiedBy { get; set; }
        public int Id { get; set; }
    }

    internal class TestRepository : AuditingRepository<AuditedEntity, int> {
        public readonly List<AuditedEntity> TestEntities;
        public TestRepository(ITimeProvider timeProvider, ICurrentUserIdProvider currentUserProvider) : base(timeProvider, currentUserProvider) {
            TestEntities = [];
        }

        public override void Add(AuditedEntity item) {
            base.Add(item); // set the auditing properties
            TestEntities.Add(item);
        }
        public override void Remove(AuditedEntity item) {
            throw new NotImplementedException();
        }

        public override Task RemoveAsync(Expression<Func<AuditedEntity, bool>> predicate, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public override Task ExecuteAsync(Expression<Func<AuditedEntity, bool>> predicate, Action<AuditedEntity> action, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public override Task<List<AuditedEntity>> GetAsync(Expression<Func<AuditedEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public override Task<List<AuditedEntity>> GetAllAsync(CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public override Task<bool> ExistsAsync(Expression<Func<AuditedEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            return Task.FromResult(TestEntities.Any(predicate.Compile()));
        }

        public override Task<int> CountAsync(Expression<Func<AuditedEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public override Task<AuditedEntity> GetSingleAsync(Expression<Func<AuditedEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }
    }

    [UnitTest, TestMethod]
    public void CanAuditOnCreate() {
        // Arrange
        var repository = new TestRepository(_timeProvider, _userProvider);
        var entity = new AuditedEntity();

        // Act
        repository.Add(entity);

        // Assert
        Assert.IsTrue(entity.CreatedBy.HasValue());
    }

    [UnitTest, TestMethod]
    public void CanAuditOnUpdate() {
        // Arrange
        var repository = new TestRepository(_timeProvider, _userProvider);
        var entity = new AuditedEntity {
            Id = 1
        };
        repository.Add(entity);

        // Act
        repository.Update(entity);

        // Assert
        Assert.IsTrue(entity.ModifiedBy.HasValue());
    }

    [UnitTest, TestMethod]
    public async Task CanAuditCreateOnUpdateOrAddAsync() {
        // Arrange
        var repository = new TestRepository(_timeProvider, _userProvider);
        var entity = new AuditedEntity { Id = 2 };

        // Act
        await repository.UpdateOrAddAsync(entity).ConfigureAwait(false);

        // Assert
        Assert.IsTrue(entity.CreatedBy.HasValue());
    }

    [UnitTest, TestMethod]
    public async Task CanAuditUpdateOnUpdateOrAddAsync() {
        // Arrange
        var repository = new TestRepository(_timeProvider, _userProvider);
        var entity = new AuditedEntity();
        repository.Add(entity);

        // Act
        await repository.UpdateOrAddAsync(entity).ConfigureAwait(false);

        // Assert
        Assert.IsTrue(entity.ModifiedBy.HasValue());
    }
}