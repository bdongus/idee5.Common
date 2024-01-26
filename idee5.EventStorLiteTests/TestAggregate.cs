using CQRSlite.Domain;
namespace idee5.EventstorLiteTests;

internal class TestAggregate : AggregateRoot {
    public string Name { get; private set; }
    public string? Description { get; private set; }
    private TestAggregate() {
    }
    public TestAggregate(Guid aggregateId, string name, string? description) {
        ApplyChange(new TestAggregateCreated(aggregateId, name, description));
    }
    private void Apply(TestAggregateCreated e) {
        Id = e.Id;
        Name = e.Name;
        Description = e.Description;
    }
    public void Describe(string? description) {
        ApplyChange(new TestAggregateDescribed(Id, description));
    }
    private void Apply(TestAggregateDescribed e) {
        Description = e.Description;
    }
}
