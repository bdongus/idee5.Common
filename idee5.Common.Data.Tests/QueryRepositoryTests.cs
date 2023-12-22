using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    /// <summary>
    /// Unit test the query repository and the querying repository extensions
    /// </summary>
    [TestClass]
    public class QueryRepositoryTests {
        internal class TransientEntity : IEntity {
            public int Id { get; set; }
            public string Workload { get; set; }
        }

        private static readonly List<TransientEntity> _transientEntities =
        [
            new TransientEntity() { Id = 1, Workload = "wl1" },
            new TransientEntity() { Id = 2, Workload = "wl2" },
            new TransientEntity() { Id = 3, Workload = "wl3" },
        ];

        internal class TransientQueryRepository : IQueryRepository<TransientEntity> {
            public Task<int> CountAsync(Expression<Func<TransientEntity, bool>> predicate, CancellationToken cancellationToken = default) {
                var result = _transientEntities.Count(predicate.Compile());
                return Task.FromResult(result);
            }

            public Task<bool> ExistsAsync(Expression<Func<TransientEntity, bool>> predicate, CancellationToken cancellationToken = default) {
                return Task.Run(() => _transientEntities.Any(predicate.Compile()), cancellationToken);
                throw new NotImplementedException();
            }

            public Task<List<TransientEntity>> GetAllAsync(CancellationToken cancellationToken = default) {
                return Task.FromResult(_transientEntities);
            }

            public Task<List<TransientEntity>> GetAsync(Expression<Func<TransientEntity, bool>> predicate, CancellationToken cancellationToken = default) {
                var result = _transientEntities.Where(predicate.Compile());
                return Task.FromResult(result.ToList());
            }

            public Task<TransientEntity> GetSingleAsync(Expression<Func<TransientEntity, bool>> predicate, CancellationToken cancellationToken = default) {
                var result = _transientEntities.SingleOrDefault(predicate.Compile());
                return Task.FromResult(result);
            }
        }

        [UnitTest, TestMethod]
        public async Task CanCountAsync() {
            // Arrange
            var repo = new TransientQueryRepository();

            // Act
            var result = await repo.CountAsync(e => e.Id != 0);

            // Assert
            Assert.AreEqual(expected: 3, actual: result);
        }

        [UnitTest, TestMethod]
        public async Task CanGetSingleAsync() {
            // Arrange
            var repo = new TransientQueryRepository();

            // Act
            TransientEntity result = await repo.GetSingleAsync(e => e.Id == 2).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: "wl2", actual: result.Workload);
        }

        [UnitTest, TestMethod]
        public async Task CanGetAsync() {
            // Arrange
            var repo = new TransientQueryRepository();

            // Act
            var result = await repo.GetAsync(e => e.Id == 2).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 1, actual: result.Count);
            Assert.AreEqual(expected: "wl2", actual: result[0].Workload);
        }

        [UnitTest, TestMethod]
        public async Task CanGetAllAsync() {
            // Arrange
            var repo = new TransientQueryRepository();

            // Act
            var result = await repo.GetAllAsync().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 3, actual: result.Count);
        }

        [UnitTest, TestMethod]
        public async Task CanCheckExistence() {
            // Arrange
            var repo = new TransientQueryRepository();

            // Act
            var fail = await repo.ExistsAsync(e => e.Id == 42);
            var success = await repo.ExistsAsync(e => e.Id == 2);

            // Assert
            Assert.IsFalse(fail);
            Assert.IsTrue(success);
        }
    }
}