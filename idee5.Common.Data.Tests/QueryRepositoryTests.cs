using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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

            public Task<int> CountAsync(Func<TransientEntity, bool> predicate, CancellationToken cancellationToken = default) {
                return Task.Run(() => _transientEntities.Count(predicate), cancellationToken);
            }

            public Task<bool> ExistsAsync(Func<TransientEntity, bool> predicate, CancellationToken cancellationToken = default) {
                return Task.Run(() => _transientEntities.Any(predicate), cancellationToken);
            }

            public Task<IEnumerable<TransientEntity>> GetAsync(Func<IQueryable<TransientEntity>, IQueryable<TransientEntity>> func, CancellationToken cancellationToken) {
                return Task.Run(() => func(_transientEntities.AsQueryable()).AsEnumerable(), cancellationToken);
            }

            public Task<TransientEntity> GetSingleAsync(Func<TransientEntity, bool> expression, CancellationToken cancellationToken = default) {
                return Task<TransientEntity>.Factory.StartNew(() => _transientEntities.SingleOrDefault(expression), cancellationToken);
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
            IEnumerable<TransientEntity> result = await repo.GetAsync(q => q.Where(e => e.Id == 2), new CancellationToken()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 1, actual: result.Count());
            Assert.AreEqual(expected: "wl2", actual: result.First().Workload);
        }

        [UnitTest, TestMethod]
        public async Task CanGetAllAsync() {
            // Arrange
            var repo = new TransientQueryRepository();

            // Act
            IEnumerable<TransientEntity> result = await repo.GetAsync(e => e, new CancellationToken()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 3, actual: result.Count());
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