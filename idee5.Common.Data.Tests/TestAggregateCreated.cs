using CQRSlite.Events;
using System;

namespace idee5.Common.Data.Tests {
    public class TestAggregateCreated : IEvent {
        public TestAggregateCreated(Guid id) {
            Id = id;
        }

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }

}
