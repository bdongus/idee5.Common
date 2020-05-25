using idee5.Common.Data.Properties;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data {
    /// <summary>
    /// Output a <see cref="ValidationResult"/> to the <see cref="Console"/>.
    /// </summary>
    public class ConsoleValidationReporter : IValidationResultReporter {
        /// <inheritdoc />
        public void Report(ValidationResult validationResult) {
            if (validationResult == null)
                throw new ArgumentNullException(nameof(validationResult));

            Console.WriteLine(String.Format(CultureInfo.InvariantCulture, Resources.MemberNames, validationResult.MemberNames.JoinAsString(",")));
            Console.WriteLine(String.Format(CultureInfo.InvariantCulture, Resources.Error, validationResult.ErrorMessage));
        }

        /// <inheritdoc />
        public Task ReportAsync(ValidationResult validationResult, CancellationToken cancellationToken = default) {
            if (validationResult == null)
                throw new ArgumentNullException(nameof(validationResult));

            Console.WriteLine(String.Format(CultureInfo.InvariantCulture, Resources.MemberNames, validationResult.MemberNames.JoinAsString(",")));
            Console.WriteLine(String.Format(CultureInfo.InvariantCulture, Resources.Error, validationResult.ErrorMessage));
            return Task.CompletedTask;
        }
    }
}
