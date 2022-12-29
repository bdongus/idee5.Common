using CQRSlite.Domain;
using CQRSlite.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class EventStoreTests {
        [TestMethod]
        public async Task CanGetEventsFromStore() {
            // Arrange
            var eventPublisher = new TestEventPublisher();
            var store = new InMemoryEventStore(eventPublisher);
            var repository = new Repository(store);
            var session = new Session(repository);
            var aggregateId = Guid.NewGuid();
            var testAggregate = new TestAggregate(aggregateId);

            // Act
            await session.Add(testAggregate).ConfigureAwait(false);
            testAggregate.DoSomething();
            testAggregate.DoSomething();
            await session.Commit().ConfigureAwait(false);
            IEnumerable<IEvent> result = await store.Get(aggregateId, 0).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1.23m, ((TestAggregateDidSomething) result.Last()).Price);
        }
    }

    public class TestAggregate : AggregateRoot {
        public TestAggregate(Guid id) {
            Id = id;
            ApplyChange(new TestAggregateCreated(id));
        }

        public int DidSomethingCount;

        public void DoSomething() {
            ApplyChange(new TestAggregateDidSomething(Id, new DateTimeRange(DateTime.Today, DateTime.Today.AddDays(29)), 1.23m));
        }

        public void Apply(TestAggregateDidSomething e) {
            DidSomethingCount++;
        }
    }
    public class TestAggregateDidSomething : IEvent {
        public Guid Id { get; set; }

        public TestAggregateDidSomething(Guid id, DateTimeRange timeRange, decimal price) {
            Id = id;
            TimeRange = timeRange;
            Price = price;
        }

        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public DateTimeRange TimeRange { get; }
        public decimal Price { get; }
    }
}
