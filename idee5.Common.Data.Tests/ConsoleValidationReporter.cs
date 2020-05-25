using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    public class ConsoleValidationReporter : IValidationResultReporter {
        public void Report(ValidationResult validationResult) {
            Console.WriteLine("Member names : " + validationResult.MemberNames.JoinAsString(","));
            Console.WriteLine("Error : " + validationResult.ErrorMessage);
        }

        public Task ReportAsync(ValidationResult validationResult, CancellationToken cancellationToken) {
            Console.WriteLine(validationResult.ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
