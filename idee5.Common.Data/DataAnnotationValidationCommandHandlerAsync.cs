using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data;
/// <summary>
/// Generic command handler decorator to support command validation. Validation is called synchronously.
/// The command is handled asynchronously.
/// </summary>
/// <typeparam name="TCommand">The type of the T command.</typeparam>
public class DataAnnotationValidationCommandHandlerAsync<TCommand> : ICommandHandlerAsync<TCommand> {
    private readonly IRecursiveAnnotationsValidator _validator;
    private readonly ICommandHandlerAsync<TCommand> _handler;
    private readonly IValidationResultReporter _validationResultReporter;

    /// <summary>
    /// The validation results.
    /// </summary>
    public List<ValidationResult> ValidationResults { get; } = new List<ValidationResult>();

    /// <summary>
    /// <c>True</c> if the validation passed successfully.
    /// </summary>
    public bool Success => ValidationResults.Count == 0;

    /// <summary>
    /// Decorate a <see cref="ICommandHandlerAsync{TCommand}"/> with a recursive data annotation validator.
    /// </summary>
    /// <param name="validator">The <see cref="IRecursiveAnnotationsValidator"/> decorating the command.</param>
    /// <param name="validationResultReporter"></param>
    /// <param name="handler">The <see cref="ICommandHandlerAsync{TCommand}"/> being decorated.</param>
    public DataAnnotationValidationCommandHandlerAsync(IRecursiveAnnotationsValidator validator, IValidationResultReporter validationResultReporter, ICommandHandlerAsync<TCommand> handler) {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _validationResultReporter = validationResultReporter ?? throw new ArgumentNullException(nameof(validationResultReporter));
    }

    /// <summary>
    /// Execute the supplied command, if it is valid.
    /// </summary>
    /// <param name="command">Command parameters.</param>
    /// <param name="cancellationToken"></param>
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken) {
        // validate the supplied command (either throws or returns false if invalid).
        if (_validator.TryValidateObjectRecursive(command, ValidationResults)) {
            cancellationToken.ThrowIfCancellationRequested();
            // forward the (valid) command to the real command handler
            await _handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        } else {
            for (int i = 0; i < ValidationResults.Count; i++) {
                cancellationToken.ThrowIfCancellationRequested();
                await _validationResultReporter.ReportAsync(ValidationResults[i], cancellationToken).ConfigureAwait(false);
            }
        }
    }
}