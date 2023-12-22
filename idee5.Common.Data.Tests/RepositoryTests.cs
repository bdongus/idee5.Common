using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests;
[TestClass]
public class RepositoryTests {
    internal class TestRepository : IRepository<TestEntity, int> {
        private readonly ITimeProvider _timeProvider;
        private readonly ICurrentUserIdProvider _currentUserIdProvider;
        public readonly List<TestEntity> TestEntities;

        public TestRepository() {
            _timeProvider = new DefaultTimeProvider();
            _currentUserIdProvider = new DefaultCurrentUserIdProvider();
            TestEntities = [
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 1, Label = "idee5", MasterSystemHierarchy = "001", MasterSystemId = "1"},
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 2, Label = "IBM", MasterSystemHierarchy = "001", MasterSystemId = "2"},
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 3, Label = "Atari", MasterSystemHierarchy = "", MasterSystemId = "3"},
                new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 4, Label = "Atari", MasterSystemHierarchy = "001", MasterSystemId = "4"},
            ];
        }

        public void Add(TestEntity item) => TestEntities.Add(item);

        public void Add(IEnumerable<TestEntity> items) {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) => Task.Run(() => TestEntities.RemoveAll(predicate.Compile()));

        public void Remove(TestEntity item) => TestEntities.Remove(item);

        public void Remove(IEnumerable<TestEntity> items) {
            throw new NotImplementedException();
        }

        public void Update(TestEntity item) {
            TestEntity listItem = TestEntities.SingleOrDefault(te => te == item);
            if (listItem != null) listItem = item;
            listItem.ModifiedBy = "me";
        }
        public void Update(IEnumerable<TestEntity> items) {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(Expression<Func<TestEntity, bool>> predicate, Action<TestEntity> action, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task UpdateOrAddAsync(TestEntity item, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task UpdateOrAddAsync(IEnumerable<TestEntity> items, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<List<TestEntity>> GetAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<List<TestEntity>> GetAllAsync(CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }

        public Task<TestEntity> GetSingleAsync(Expression<Func<TestEntity, bool>> predicate, CancellationToken cancellationToken = default) {
            var result = TestEntities.SingleOrDefault(predicate.Compile());
            return Task.FromResult(result);
        }
    }

#pragma warning disable CS0618 // Typ oder Element ist veraltet
    private readonly AmbientTimeProvider _timeProvider;
    private readonly AmbientUserProvider _currentUserIdProvider;

    public RepositoryTests() {
        _timeProvider = new AmbientTimeProvider();
        _currentUserIdProvider = new AmbientUserProvider();
    }
#pragma warning restore CS0618 // Typ oder Element ist veraltet

    [UnitTest, TestMethod]
    public void CanAdd() {
        // Arrange
        var _testRepository = new TestRepository();

        var itemToAdd = new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" };

        // Act
        _testRepository.Add(itemToAdd);

        // Assert
        Assert.AreEqual(5, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public async Task CanRemove() {
        // Arrange
        var _testRepository = new TestRepository();
        var itemToRemove = await _testRepository.GetSingleAsync(e => e.Id==4).ConfigureAwait(false);

        // Act
        _testRepository.Remove(itemToRemove);

        // Assert
        Assert.AreEqual(3, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public async Task CanRemoveByLambda() {
        // Arrange
        var _testRepository = new TestRepository();

        // Act
        // Delete the to "Atari" rows
        await _testRepository.RemoveAsync(r => r.Label == "Atari", default).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(2, _testRepository.TestEntities.Count);
    }

    [UnitTest, TestMethod]
    public async Task CanUpdate() {
        // Arrange
        var _testRepository = new TestRepository();
        var itemToUpdate = await _testRepository.GetSingleAsync(r => r.Id == 2).ConfigureAwait(false);

        // Act
        _testRepository.Update(itemToUpdate);

        // Assert
        Assert.AreEqual("me", itemToUpdate.ModifiedBy);
    }

    [UnitTest, TestMethod]
    public async Task CanUpdateOrAdd() {
        // Arrange
        var _testRepository = new TestRepository();
        var itemToAdd = new TestEntity(_timeProvider, _currentUserIdProvider) { Id = 42, Label = "toAdd", MasterSystemHierarchy = "001", MasterSystemId = "42" };
        var itemToUpdate = await _testRepository.GetSingleAsync(r => r.Id == 2).ConfigureAwait(false);

        // Act
        _testRepository.Add(itemToAdd);
        _testRepository.Update(itemToUpdate);

        // Assert
        Assert.AreEqual(5, _testRepository.TestEntities.Count);
        Assert.AreEqual("me", itemToUpdate.ModifiedBy);
    }
}