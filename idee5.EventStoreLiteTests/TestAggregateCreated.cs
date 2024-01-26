using idee5.EventStoreLite;
namespace idee5.EventstorLiteTests;

internal class TestAggregateCreated : CqrsEvent {
    public TestAggregateCreated(Guid id, string name, string? description) : base(id) {
        Name = name;
        Description = description;
    }

    public string Name { get; }
    public string? Description { get; }
}
