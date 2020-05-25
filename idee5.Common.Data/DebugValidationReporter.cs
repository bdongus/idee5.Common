using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    /// <summary>
    /// Report a <see cref="ValidationResult"/> to the <see cref="Debug"/> output.
    /// </summary>
    public class DebugValidationReporter : IValidationResultReporter {
        public void Report(ValidationResult validationResult) {
            if (validationResult == null)
                throw new System.ArgumentNullException(nameof(validationResult));

            Debug.WriteLine("Member names : " + validationResult.MemberNames.JoinAsString(","));
            Debug.WriteLine("Error : " + validationResult.ErrorMessage);
        }

        public Task ReportAsync(ValidationResult validationResult, CancellationToken cancellationToken = default) {
            if (validationResult == null)
                throw new System.ArgumentNullException(nameof(validationResult));

            Debug.WriteLine("Member names : " + validationResult.MemberNames.JoinAsString(","));
            Debug.WriteLine("Error : " + validationResult.ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
