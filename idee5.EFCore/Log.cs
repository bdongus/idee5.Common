using Microsoft.Extensions.Logging;

namespace idee5.EFCore;
internal static partial class Log {
    [LoggerMessage(1, LogLevel.Information, "Concurrency conflict occurred for {entity}")]
    public static partial void ConcurrencyConflict(this ILogger logger, string entity);

}
