using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests;

public class TestAsyncInputHandler : IAsyncInputHandler<TestInputQuery, TestEntity> {
    public async IAsyncEnumerable<TestEntity> HandleAsync(TestInputQuery query, [EnumeratorCancellation]  CancellationToken cancellationToken = default) {
        var result = new TestEntityResult();

        var input = new List<string>() { "001,1,Bernd", "001,2,Dongus", ",3,idee5", "002,4," };
        // convert to DTOs
        IEnumerable<TestEntity> imported = input.Select(i => i.Split(','))
            .Where(i2 => i2[0] == query.MasterSystemHierarchy)
            .Select(i3 => new TestEntity(new DefaultTimeProvider(), new DefaultCurrentUserIdProvider()) { Label = i3[2], MasterSystemHierarchy = i3[0], MasterSystemId = i3[1] });
        await Task.Delay(1000, cancellationToken);
        foreach (var i4 in imported)
            yield return i4;
    }
}