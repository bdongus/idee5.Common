using idee5.EventStoreLite;
namespace idee5.EventstorLiteTests;

internal class TestAggregateDescribed : CqrsEvent {
    public string? Description { get; private set; }
    public TestAggregateDescribed(Guid id, string? description) : base(id) {
        Description = description;
    }
}
