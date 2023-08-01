using CQRSlite.Domain;
using CQRSlite.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var aggregate = await session.Get<TestAggregate>(aggregateId).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1.23m, ((TestAggregateDidSomething)result.Last()).Price);
            Assert.AreEqual(2.46m, aggregate.Price);
        }
    }

    public class TestAggregate : AggregateRoot {
        private TestAggregate() {

        }
        public decimal Price { get; private set; }
        public TestAggregate(Guid id) {
            Id = id;
            ApplyChange(new TestAggregateCreated(id));
        }

        public void DoSomething() {
            ApplyChange(new TestAggregateDidSomething(Id, new DateTimeRange(DateTime.Today, DateTime.Today.AddDays(29)), 1.23m));
        }

        public void Apply(TestAggregateDidSomething e) {
            Price += e.Price;
        }
    }

    public record TestAggregateDidSomething(Guid Id, DateTimeRange TimeRange, decimal Price) : IEvent {
        public Guid Id { get; set; } = Id;
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
