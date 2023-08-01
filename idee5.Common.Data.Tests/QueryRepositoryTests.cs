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

        private static readonly IList<TransientEntity> _transientEntities = new List<TransientEntity>
        {
            new TransientEntity() { Id = 1, Workload = "wl1" },
            new TransientEntity() { Id = 2, Workload = "wl2" },
            new TransientEntity() { Id = 3, Workload = "wl3" },
        };

        internal class TransientQueryRepository : IQueryRepository<TransientEntity> {
            public Task<IEnumerable<TransientEntity>> GetAsync(Func<IQueryable<TransientEntity>, IQueryable<TransientEntity>> func, CancellationToken cancellationToken) {
                return Task.Run(() => func(_transientEntities.AsQueryable()).AsEnumerable(), cancellationToken);
            }

            public Task<TResult> GetAsync<TResult>(Func<IQueryable<TransientEntity>, TResult> func, CancellationToken cancellationToken) {
                return Task<TResult>.Factory.StartNew(() => func(_transientEntities.AsQueryable()), cancellationToken);
            }
        }

        [UnitTest, TestMethod]
        public async Task CanCountAsync() {
            // Arrange
            var repo = new TransientQueryRepository();

            // Act
            var result = await repo.GetAsync(e => e.Count(), new CancellationToken()).ConfigureAwait(false);

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
            IEnumerable<TransientEntity> result = await repo.GetAllAsync(new CancellationToken()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 3, actual: result.Count());
        }

        [UnitTest, TestMethod]
        public async Task CanFindAsync() {
            // Arrange
            var repo = new TransientQueryRepository();

            // Act
            IEnumerable<TransientEntity> result = await repo.FindByAsync(e => e.Id == 3).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 3, actual: result.First().Id);
        }

        [UnitTest, TestMethod]
        public async Task CanPaginateAsync() {
            // Arrange
            var repo = new TransientQueryRepository();

            // Act
            PagedCollection<TransientEntity> result = await repo.PaginateAsync(q => q.Where(e => e.Workload.StartsWith("wl")), 0, 2).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 2, actual: result.Count);
            Assert.AreEqual(expected: 3, actual: result.TotalCount);
        }
    }
}