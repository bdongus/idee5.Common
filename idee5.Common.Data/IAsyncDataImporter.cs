using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// The asynchronous data importer
/// </summary>
/// <typeparam name="TQuery">The query parameter type</typeparam>
/// <typeparam name="TCommand">The command parameter type</typeparam>
/// <typeparam name="TCleanupCmd">The cleanup command parameter type</typeparam>
public interface IAsyncDataImporter<TQuery, TCommand, TCleanupCmd> where TQuery : IQuery<TCommand> where TCleanupCmd : ICleanupCommand {
    /// <summary>
    /// Executes the import
    /// </summary>
    /// <param name="query">The query parameters</param>
    /// <param name="cleanupCmd">The cleanup parameters</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task ExecuteAsync(TQuery query, TCleanupCmd cleanupCmd, CancellationToken cancellationToken = default);
}