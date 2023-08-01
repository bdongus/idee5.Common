using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class ARepositoryTests {
        internal class MeUserProvider : ICurrentUserIdProvider {
            public string GetCurrentUserId() => "me";
        }

        internal class TestRepository : ARepository<TestEntity, int> {
            private readonly AmbientTimeProvider _timeProvider;
            private readonly AmbientUserProvider _currentUserIdProvider;
            private readonly IList<TestEntity> _testEntities;

            public TestRepository(AmbientTimeProvider timeProvider, AmbientUserProvider currentUserIdProvider) {
                _timeProvider = timeProvider;
                _currentUserIdProvider = currentUserIdProvider;
                _currentUserIdProvider.Instance = new MeUserProvider();
                _testEntities = new List<TestEntity> {
                    new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 1, Label = "idee5", MasterSystemHierarchy = "001", MasterSystemId = "1"},
                    new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 2, Label = "IBM", MasterSystemHierarchy = "001", MasterSystemId = "2"},
                    new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 3, Label = "Atari", MasterSystemHierarchy = "", MasterSystemId = "3"},
                    new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 4, Label = "Atari", MasterSystemHierarchy = "001", MasterSystemId = "4"},
                };
            }

            public override void Add(TestEntity item) => _testEntities.Add(item);

            public override Task<IEnumerable<TestEntity>> GetAsync(Func<IQueryable<TestEntity>, IQueryable<TestEntity>> func, CancellationToken cancellationToken = default) {
                return Task.Factory.StartNew(() => func(_testEntities.AsQueryable()).AsEnumerable(), cancellationToken);
            }

            public override Task<TResult> GetAsync<TResult>(Func<IQueryable<TestEntity>, TResult> func, CancellationToken cancellationToken = default) {
                return Task<TResult>.Factory.StartNew(() => func(_testEntities.AsQueryable()), cancellationToken);
            }

            public override void Remove(TestEntity item) => _testEntities.Remove(item);

            public override Task RemoveAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) => Task.Factory.StartNew(() => _testEntities.RemoveAll(predicate.Compile()));

            public override void Update(TestEntity item) {
                TestEntity listItem = _testEntities.SingleOrDefault(te => te == item);
                if (listItem != null) listItem = item;
                listItem.ModifiedBy = _currentUserIdProvider.GetCurrentUserId();
            }

            public override Task ExecuteAsync(Expression<Func<TestEntity, bool>> predicate, Action<TestEntity> action, CancellationToken cancellationToken = default) {
                throw new NotImplementedException();
            }

            public override Task UpdateOrAddAsync(TestEntity item, CancellationToken cancellationToken = default) {
                TestEntity listItem = _testEntities.SingleOrDefault(te => te == item);
                if (listItem != null) {
                    listItem = item;
                    listItem.ModifiedBy = _currentUserIdProvider.GetCurrentUserId();
                } else {
                    _testEntities.Add(item);
                }
                return Task.CompletedTask;
            }
        }

        // Default providers
        private readonly AmbientTimeProvider _timeProvider;
        private readonly AmbientUserProvider _currentUserIdProvider;

        public ARepositoryTests() {
            _timeProvider = new AmbientTimeProvider();
            _currentUserIdProvider = new AmbientUserProvider();
        }

        [UnitTest, TestMethod]
        public async Task CanAdd() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

            var itemToAdd = new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" };

            // Act
            _testRepository.Add(itemToAdd);

            // Assert
            int result = await _testRepository.GetAsync(q => q.Count()).ConfigureAwait(false);
            Assert.AreEqual(5, result);
        }

        [UnitTest, TestMethod]
        public async Task CanIgnoreNullListOnAdd() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
            IEnumerable<TestEntity> items = null;
            int expectedCount = await _testRepository.GetAsync(e => e.Count(), new CancellationToken()).ConfigureAwait(false);

            // Act
            _testRepository.Add(items);
            int result = await _testRepository.GetAsync(e => e.Count(), new CancellationToken()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expectedCount, result);
        }

        [UnitTest, TestMethod]
        public async Task CanAddMultiple() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

            TestEntity[] listOfItems = {
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" },
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 43, Label = "toAdd2", MasterSystemHierarchy = "001", MasterSystemId = "43" }
            };

            // Act
            _testRepository.Add(listOfItems);

            // Assert
            int result = await _testRepository.GetAsync(q => q.Count()).ConfigureAwait(false);
            Assert.AreEqual(6, result);
        }

        [UnitTest, TestMethod]
        public async Task CanRemove() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
            TestEntity itemToRemove = (await _testRepository.GetAllAsync().ConfigureAwait(false)).Last();

            // Act
            _testRepository.Remove(itemToRemove);

            // Assert
            int result = await _testRepository.GetAsync(q => q.Count()).ConfigureAwait(false);
            Assert.AreEqual(3, result);
        }

        [UnitTest, TestMethod]
        public async Task CanIgnoreNullListOnRemove() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
            IEnumerable<TestEntity> items = null;
            int expectedCount = await _testRepository.GetAsync(e => e.Count(), new CancellationToken()).ConfigureAwait(false);

            // Act
            _testRepository.Remove(items);
            int result = await _testRepository.GetAsync(e => e.Count(), new CancellationToken()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expectedCount, result);
        }

        [UnitTest, TestMethod]
        public async Task CanRemoveMultiple() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

            var listOfItems = new List<TestEntity> {
                (await _testRepository.GetAllAsync().ConfigureAwait(false)).Last(),
                (await _testRepository.GetAllAsync().ConfigureAwait(false)).First()
            };
            // Act
            _testRepository.Remove(listOfItems);

            // Assert
            int result = await _testRepository.GetAsync(q => q.Count()).ConfigureAwait(false);
            Assert.AreEqual(2, result);
        }

        [UnitTest, TestMethod]
        public async Task CanRemoveByLambda() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

            // Act
            // Delete the to "Atari" rows
            await _testRepository.RemoveAsync(r => r.Label == "Atari", default).ConfigureAwait(false);

            // Assert
            int result = await _testRepository.GetAsync(q => q.Count()).ConfigureAwait(false);
            Assert.AreEqual(2, result);
        }

        [UnitTest, TestMethod]
        public async Task CanUpdate() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
            TestEntity itemToUpdate = await _testRepository.GetSingleAsync(r => r.Id == 2).ConfigureAwait(false);

            // Act
            _testRepository.Update(itemToUpdate);

            // Assert
            Assert.AreEqual("me", itemToUpdate.ModifiedBy);
        }

        [UnitTest, TestMethod]
        public async Task CanIgnoreNullListOnUpdate() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
            IEnumerable<TestEntity> items = null;
            int expectedCount = await _testRepository.GetAsync(e => e.Count(), new CancellationToken()).ConfigureAwait(false);

            // Act
            _testRepository.Update(items);
            int result = await _testRepository.GetAsync(e => e.Count(), new CancellationToken()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expectedCount, result);
        }

        [UnitTest, TestMethod]
        public async Task CanUpdateMultiple() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

            var listOfItems = new List<TestEntity> {
                (await _testRepository.GetAllAsync().ConfigureAwait(false)).Last(),
                (await _testRepository.GetAllAsync().ConfigureAwait(false)).First()
            };
            // Act
            _testRepository.Update(listOfItems);

            // Assert
            int result = await _testRepository.GetAsync(q => q.Count()).ConfigureAwait(false);
            Assert.AreEqual(4, result);
            Assert.AreEqual(2, listOfItems.Count(i => i.ModifiedBy == "me"));
        }

        [UnitTest, TestMethod]
        public async Task CanUpdateOrAdd() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
            var itemToAdd = new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" };
            TestEntity itemToUpdate = await _testRepository.GetSingleAsync(r => r.Id == 2).ConfigureAwait(false);

            // Act
            _testRepository.Add(itemToAdd);
            _testRepository.Update(itemToUpdate);

            // Assert
            int result = await _testRepository.GetAsync(q => q.Count()).ConfigureAwait(false);
            Assert.AreEqual(5, result);
            Assert.AreEqual("me", itemToUpdate.ModifiedBy);
        }

        [UnitTest, TestMethod]
        public async Task CanIgnoreNullListOnUpdateOrAddAsync() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
            IEnumerable<TestEntity> items = null;
            int expectedCount = await _testRepository.GetAsync(e => e.Count(), new CancellationToken()).ConfigureAwait(false);

            // Act
            await _testRepository.UpdateOrAddAsync(items).ConfigureAwait(false);
            int result = await _testRepository.GetAsync(e => e.Count(), new CancellationToken()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expectedCount, result);
        }

        [UnitTest, TestMethod]
        public async Task CanUpdateOrAddAsyncMultiple() {
            // Arrange
            IRepository<TestEntity, int> _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
            var listOfItems = new List<TestEntity> {
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" },
                await _testRepository.GetSingleAsync(r => r.Id == 2).ConfigureAwait(false)
            };

            // Act
            await _testRepository.UpdateOrAddAsync(listOfItems).ConfigureAwait(false);
            // Assert
            int result = await _testRepository.GetAsync(q => q.Count()).ConfigureAwait(false);
            Assert.AreEqual(5, result);
            Assert.AreEqual(1, listOfItems.Count(i => i.ModifiedBy == "me"));
        }
    }
}
