using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests;
[TestClass]
public class ARepositoryTests {
    internal class MeUserProvider : ICurrentUserIdProvider {
        public string GetCurrentUserId() => "me";
    }

#pragma warning disable CS0618 // Typ oder Element ist veraltet
    internal class TestRepository : ARepository<TestEntity, int> {
        private readonly AmbientTimeProvider _timeProvider;
        private readonly AmbientUserProvider _currentUserIdProvider;
        public readonly List<TestEntity> TestEntities;

        public TestRepository(AmbientTimeProvider timeProvider, AmbientUserProvider currentUserIdProvider) {
            _timeProvider = timeProvider;
            _currentUserIdProvider = currentUserIdProvider;
            _currentUserIdProvider.Instance = new MeUserProvider();
            TestEntities = [
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 1, Label = "idee5", MasterSystemHierarchy = "001", MasterSystemId = "1"},
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 2, Label = "IBM", MasterSystemHierarchy = "001", MasterSystemId = "2"},
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 3, Label = "Atari", MasterSystemHierarchy = "", MasterSystemId = "3"},
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 4, Label = "Atari", MasterSystemHierarchy = "001", MasterSystemId = "4"},
            ];
        }

        public override void Add(TestEntity item) => TestEntities.Add(item);

        public override void Remove(TestEntity item) => TestEntities.Remove(item);

        public override Task RemoveAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            TestEntities.RemoveAll(predicate.Compile());
            return Task.CompletedTask;
        }

        public override void Update(TestEntity item) {
            TestEntity listItem = TestEntities.SingleOrDefault(te => te == item);
            if (listItem != null) listItem = item;
            listItem.ModifiedBy = _currentUserIdProvider.GetCurrentUserId();
        }

        public override Task ExecuteAsync(Expression<Func<TestEntity, bool>> predicate, Action<TestEntity> action, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public override Task UpdateOrAddAsync(TestEntity item, CancellationToken cancellationToken = default) {
            TestEntity listItem = TestEntities.SingleOrDefault(te => te == item);
            if (listItem != null) {
                listItem = item;
                listItem.ModifiedBy = _currentUserIdProvider.GetCurrentUserId();
            } else {
                TestEntities.Add(item);
            }
            return Task.CompletedTask;
        }

        public override Task<List<TestEntity>> GetAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public override Task<List<TestEntity>> GetAllAsync(CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public override Task<bool> ExistsAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            var result = TestEntities.Any(predicate.Compile());
            return Task.FromResult(result);
        }

        public override Task<int> CountAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            var result = TestEntities.Count(predicate.Compile());
            return Task.FromResult(result);
        }

        public override Task<TestEntity> GetSingleAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            var result = TestEntities.SingleOrDefault(predicate.Compile());
            return Task.FromResult(result);
        }
    }

    // Default providers
    private readonly AmbientTimeProvider _timeProvider;
    private readonly AmbientUserProvider _currentUserIdProvider;

    public ARepositoryTests() {
        _timeProvider = new AmbientTimeProvider();
        _currentUserIdProvider = new AmbientUserProvider();
    }
#pragma warning restore CS0618 // Typ oder Element ist veraltet

    [UnitTest, TestMethod]
    public void CanAdd() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

        var itemToAdd = new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" };

        // Act
        _testRepository.Add(itemToAdd);

        // Assert
        int result = _testRepository.TestEntities.Count;
        Assert.AreEqual(5, result);
    }

    [UnitTest, TestMethod]
    public void CanIgnoreNullListOnAdd() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
        IEnumerable<TestEntity> items = null;
        int expectedCount = _testRepository.TestEntities.Count;

        // Act
        _testRepository.Add(items);
        int result = _testRepository.TestEntities.Count;

        // Assert
        Assert.AreEqual(expectedCount, result);
    }

    [UnitTest, TestMethod]
    public void CanAddMultiple() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

        TestEntity[] listOfItems = [
            new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" },
            new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 43, Label = "toAdd2", MasterSystemHierarchy = "001", MasterSystemId = "43" }
        ];

        // Act
        _testRepository.Add(listOfItems);

        // Assert
        Assert.AreEqual(6, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public async Task CanRemove() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
        TestEntity itemToRemove = await _testRepository.GetSingleAsync(e => e.Id == 1).ConfigureAwait(false);

        // Act
        _testRepository.Remove(itemToRemove);

        // Assert
        Assert.AreEqual(3, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public void CanIgnoreNullListOnRemove() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
        IEnumerable<TestEntity> items = null;
        int expectedCount = _testRepository.TestEntities.Count;

        // Act
        _testRepository.Remove(items);

        // Assert
        Assert.AreEqual(expectedCount, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public async Task CanRemoveMultiple() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

        var listOfItems = new List<TestEntity> {
            await _testRepository.GetSingleAsync(e => e.Id == 1),
            await _testRepository.GetSingleAsync(e => e.Id == 4)
        };
        // Act
        _testRepository.Remove(listOfItems);

        // Assert
        Assert.AreEqual(2, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public async Task CanRemoveByLambda() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

        // Act
        // Delete the to "Atari" rows
        await _testRepository.RemoveAsync(r => r.Label == "Atari", default).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(2, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public async Task CanUpdate() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
        TestEntity itemToUpdate = await _testRepository.GetSingleAsync(r => r.Id == 2).ConfigureAwait(false);

        // Act
        _testRepository.Update(itemToUpdate);

        // Assert
        Assert.AreEqual("me", itemToUpdate.ModifiedBy);
    }

    [UnitTest, TestMethod]
    public void CanIgnoreNullListOnUpdate() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
        IEnumerable<TestEntity> items = null;
        int expectedCount = _testRepository.TestEntities.Count;

        // Act
        _testRepository.Update(items);

        // Assert
        Assert.AreEqual(expectedCount, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public async Task CanUpdateMultiple() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);

        var listOfItems = new List<TestEntity> {
            await _testRepository.GetSingleAsync(e => e.Id == 1),
            await _testRepository.GetSingleAsync(e => e.Id == 4)
        };
        // Act
        _testRepository.Update(listOfItems);

        // Assert
        int result = _testRepository.TestEntities.Count;
        Assert.AreEqual(4, result);
        Assert.AreEqual(2, listOfItems.Count(i => i.ModifiedBy == "me"));
    }

    [UnitTest, TestMethod]
    public async Task CanUpdateOrAdd() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
        var itemToAdd = new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" };
        TestEntity itemToUpdate = await _testRepository.GetSingleAsync(r => r.Id == 2).ConfigureAwait(false);

        // Act
        _testRepository.Add(itemToAdd);
        _testRepository.Update(itemToUpdate);

        // Assert
        Assert.AreEqual(5, _testRepository.TestEntities.Count);
        Assert.AreEqual("me", itemToUpdate.ModifiedBy);
    }

    [UnitTest, TestMethod]
    public async Task CanIgnoreNullListOnUpdateOrAddAsync() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
        IEnumerable<TestEntity> items = null;
        int expectedCount = _testRepository.TestEntities.Count;

        // Act
        await _testRepository.UpdateOrAddAsync(items).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedCount, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public async Task CanUpdateOrAddAsyncMultiple() {
        // Arrange
        var _testRepository = new TestRepository(_timeProvider, _currentUserIdProvider);
        var listOfItems = new List<TestEntity> {
            new(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" },
            await _testRepository.GetSingleAsync(r => r.Id == 2).ConfigureAwait(false)
        };

        // Act
        await _testRepository.UpdateOrAddAsync(listOfItems).ConfigureAwait(false);
        // Assert
        Assert.AreEqual(5, _testRepository.TestEntities.Count);
        Assert.AreEqual(1, listOfItems.Count(i => i.ModifiedBy == "me"));
    }
}
