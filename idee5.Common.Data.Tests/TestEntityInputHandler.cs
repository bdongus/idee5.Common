using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    public class TestEntityInputHandler : IQueryHandlerAsync<InputQuery, TestEntityResult> {
        public Task<TestEntityResult> HandleAsync(InputQuery query, CancellationToken cancellationToken) {
            var result = new TestEntityResult();

            var input = new List<string>() { "001,1,Bernd", "001,2,Dongus", ",3,idee5", "002,4," };
            // convert to DTOs
            IEnumerable<TestEntity> imported = input.Select(i => i.Split(','))
                .Where(i2 => i2[0] == query.MasterSystemHierarchy)
                .Select(i3 => new TestEntity(new DefaultTimeProvider(), new DefaultCurrentUserIdProvider()) { Label = i3[2], MasterSystemHierarchy = i3[0], MasterSystemId = i3[1] });
            return Task.Run(() => { result.Entities = new PagedCollection<TestEntity>(imported, imported.Count()); return result; }, cancellationToken);
        }
    }
}