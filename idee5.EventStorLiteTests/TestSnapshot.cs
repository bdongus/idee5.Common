using CQRSlite.Snapshotting;
using idee5.Common.Data;
namespace idee5.EventstorLiteTests;
public partial class SQLiteInMemTests {
    internal class TestSnapshot : Snapshot, IGuidEntity {
        public string Name { get; set; }
        public string? Description { get; set; }

        public TestSnapshot() {
            Name = "";
        }
        public TestSnapshot(TestAggregate aggregate) {
            if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

            Id = aggregate.Id;
            Version = aggregate.Version;
            Name = aggregate.Name;
        }
    }
}
