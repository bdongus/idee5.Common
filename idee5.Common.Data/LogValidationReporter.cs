using idee5.Common.Data.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// Report a <see cref="ValidationResult"/> to the <see cref="ILogger"/> output.
/// </summary>
public class LogValidationReporter : IValidationResultReporter {
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogValidationReporter"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public LogValidationReporter(ILogger logger) {
        _logger = logger ?? NullLogger.Instance;
    }

    /// <inheritdoc/>
    public void Report(ValidationResult validationResult) {
        if (validationResult == null) {
            _logger.LogWarning(Resources.NoValidationResult);
        } else {
            _logger.LogError(Resources.MemberNames, validationResult.MemberNames.JoinAsString(","));
            _logger.LogError(Resources.Error, validationResult.ErrorMessage);
        }
    }

    /// <inheritdoc/>
    public Task ReportAsync(ValidationResult validationResult, CancellationToken cancellationToken = default) {
        if (validationResult == null) {
            _logger.LogWarning(Resources.NoValidationResult);
        } else {
            _logger.LogError(Resources.MemberNames, validationResult.MemberNames.JoinAsString(","));
            _logger.LogError(Resources.Error, validationResult.ErrorMessage);
        }
        return Task.CompletedTask;
    }
}
