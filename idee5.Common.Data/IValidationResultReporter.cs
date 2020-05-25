using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data {
    /// <summary>
    /// <see cref="ValidationResult"/> output handler.
    /// </summary>
    public interface IValidationResultReporter {
        /// <summary>
        /// Report the validation result synchronously.
        /// </summary>
        /// <param name="validationResult">The <see cref="ValidationResult"/> to report/output.</param>
        void Report(ValidationResult validationResult);

        /// <summary>
        /// Report the validation result synchronously.
        /// </summary>
        /// <param name="validationResult">The <see cref="ValidationResult"/> to report/output.</param>
        /// <param name="cancellationToken">Token to cancel to operation.</param>
        /// <returns>An awaitable Task.</returns>
        Task ReportAsync(ValidationResult validationResult, CancellationToken cancellationToken = default);
    }
}
