using System.Threading;
using System.Threading.Tasks;

// for asynchronous construction  please use the factory pattern or AsyncLazy from the AsyncExPackage
// http://blog.stephencleary.com/2013/01/async-oop-2-constructors.html
namespace idee5.Common;
/// <summary>
/// Command handler interface.
/// </summary>
/// <remarks>https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=91</remarks>
/// <typeparam name="TCommand">Command parameter(s).</typeparam>
public interface ICommandHandlerAsync<TCommand> {
    /// <summary>
    /// Handle/execute the command asynchronusly.
    /// </summary>
    /// <param name="command">The command parameter(s).</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The command <see cref="Task"/></returns>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}