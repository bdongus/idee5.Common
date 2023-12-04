using idee5.Common.Data.Properties;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// Report a <see cref="ValidationResult"/> to the <see cref="Debug"/> output.
/// </summary>
public class DebugValidationReporter : IValidationResultReporter {
    /// <inheritdoc/>
    public void Report(ValidationResult validationResult) {
#if NETSTANDARD2_0_OR_GREATER
        if (validationResult == null) throw new ArgumentNullException(nameof(validationResult));
#else
        ArgumentNullException.ThrowIfNull(validationResult);
#endif
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Resources.MemberNames, validationResult.MemberNames.JoinAsString(",")));
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Resources.Error, validationResult.ErrorMessage));
    }

    /// <inheritdoc/>
    public Task ReportAsync(ValidationResult validationResult, CancellationToken cancellationToken = default) {
#if NETSTANDARD2_0_OR_GREATER
        if (validationResult == null) throw new ArgumentNullException(nameof(validationResult));
#else
        ArgumentNullException.ThrowIfNull(validationResult);
#endif
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Resources.MemberNames, validationResult.MemberNames.JoinAsString(",")));
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Resources.Error, validationResult.ErrorMessage));
        return Task.CompletedTask;
    }
}
