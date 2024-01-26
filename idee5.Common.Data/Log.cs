using Microsoft.Extensions.Logging;
using System;

namespace idee5.Common.Data;

internal static partial class Log {
    [LoggerMessage(1, LogLevel.Information, "Import {inputQueryName} started at {startTime} (UTC).")]
    public static partial void ImportStarted(this ILogger logger, string inputQueryName, DateTime startTime);

    [LoggerMessage(2, LogLevel.Information, "Cleaning up data with {cleanUpCmdName} started at {startTime} (UTC).")]
    public static partial void CleaningUpImport(this ILogger logger, string cleanUpCmdName, DateTime startTime);
    [LoggerMessage(3, LogLevel.Information, "Import of {inputQueryName} finishes at {EndTime} (UTC). {importCounter} rows processed.")]
    public static partial void ImportFinished(this ILogger logger, string inputQueryName, DateTime endTime, int importCounter);
    [LoggerMessage(4, LogLevel.Debug, "Importing row {rowNumber}: {rowData}")]
    public static partial void Importing(this ILogger logger, int rowNumber, string rowData);
    [LoggerMessage(5, LogLevel.Error, "Row with invalid data detected: {errorMessage}")]
    public static partial void ImportError(this ILogger logger, string errorMessage);
}
