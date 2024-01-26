using System.Threading;
using System.Collections.Generic;

namespace idee5.Common.Data;

/// <summary>
/// The input handler interface.
/// </summary>
/// <typeparam name="TQuery">The query/input handler parameters.</typeparam>
/// <typeparam name="TResult">The resulting type. For the data converter it should be the command type.</typeparam>
public interface IAsyncInputHandler<TQuery, TResult> where TQuery : IQuery<TResult> {
    /// <summary>
    /// Handle the query call asynchronously.
    /// </summary>
    /// <param name="query">query parameter(s).</param>
    /// <param name="cancellationToken">Token to handle cancellation.</param>
    IAsyncEnumerable<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
