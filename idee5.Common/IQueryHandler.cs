namespace idee5.Common;
/// <summary>
/// Query pattern: https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=92
/// </summary>
/// <typeparam name="TQuery">An <see cref="IQuery{TResult}"/></typeparam>
/// <typeparam name="TResult">The result data type.</typeparam>
public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult> {
    /// <summary>
    /// Handle the given query.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>The query result.</returns>
    TResult Handle(TQuery query);
}