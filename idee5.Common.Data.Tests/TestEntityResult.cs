using System.Collections.Generic;

namespace idee5.Common.Data.Tests {
    public class TestEntityResult {
        public PagedCollection<TestEntity> Entities { get; set; } = new PagedCollection<TestEntity>(new List<TestEntity>(), 0);
    }
}