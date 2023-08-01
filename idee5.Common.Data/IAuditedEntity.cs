using System;

namespace idee5.Common.Data;
/// <summary>
/// Adds auditing properties to an entity.
/// </summary>
public interface IAuditedEntity {
    /// <summary>
    /// UTC-Timestamp of the entity's creation
    /// </summary>
    /// <remarks>Best set via a <see cref="ITimeProvider"/>.</remarks>
    DateTime DateCreatedUTC { get; set; }

    /// <summary>
    /// Identification who created this instance.
    /// </summary>
    /// <remarks>Best set via a <see cref="ICurrentUserIdProvider"/>.</remarks>
    string CreatedBy { get; set; }

    /// <summary>
    /// UTC-Timestamp of last modification
    /// </summary>
    /// <remarks>Best set via a <see cref="ITimeProvider"/>.</remarks>
    DateTime? DateModifiedUTC { get; set; }

    /// <summary>
    /// Last one modifiying this instance.
    /// </summary>
    /// <remarks>Best set via a <see cref="ICurrentUserIdProvider"/>.</remarks>
    string? ModifiedBy { get; set; }
}