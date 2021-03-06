using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class AuditingRepositoryTests {
        private readonly AmbientTimeProvider _timeProvider = new AmbientTimeProvider();
        private readonly AmbientUserProvider _userProvider = new AmbientUserProvider();
        internal class AuditedEntity : IAuditedEntity, IEntity {
            public DateTime DateCreated { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? DateModified { get; set; }
            public string ModifiedBy { get; set; }
            public int Id { get; set; }
        }

        internal class TestRepository : AuditingRepository<AuditedEntity, int> {
            private readonly List<AuditedEntity> _testEntities;
            public TestRepository(ITimeProvider timeProvider, ICurrentUserIdProvider currentUserProvider) : base(timeProvider, currentUserProvider) {
                _testEntities = new List<AuditedEntity>();
            }

            public override Task<IEnumerable<AuditedEntity>> GetAsync(Func<IQueryable<AuditedEntity>, IQueryable<AuditedEntity>> func, CancellationToken cancellationToken = default) {
                return Task.Run(() => func(_testEntities.AsQueryable()).AsEnumerable(), cancellationToken);
            }

            public override Task<TResult> GetAsync<TResult>(Func<IQueryable<AuditedEntity>, TResult> func, CancellationToken cancellationToken = default) {
                //var target = (AuditedEntity)func.Target.GetType().GetFields()[0].GetValue(func.Target);
                //if (target.Id == 2)
                //    return Task.FromResult(func.Invoke(Array.Empty<AuditedEntity>().AsQueryable()));
                //else
                //    return Task.FromResult(func.Invoke(new List<AuditedEntity> { target }.AsQueryable()));
                return Task.Run(() => func(_testEntities.AsQueryable()), cancellationToken);
            }

            public override void Add(AuditedEntity item) {
                base.Add(item); // set the auditing properties
                _testEntities.Add(item);

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
            var entity = new AuditedEntity
            {
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
}