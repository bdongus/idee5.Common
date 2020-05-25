using System;

namespace idee5.Common.Data {
    /// <summary>
    /// Adds auditing properties to an entity.
    /// </summary>
    public interface IAuditedEntity
    {
        /// <summary>
        /// Date and time of the entity's creation. Usually in UTC.
        /// </summary>
        /// <remarks>Best set via a <see cref="ITimeProvider"/>.</remarks>
        DateTime DateCreated { get; set; }

        /// <summary>
        /// Identification who created this instance.
        /// </summary>
        /// <remarks>Best set via a <see cref="ICurrentUserIdProvider"/>.</remarks>
        string CreatedBy { get; set; }

        /// <summary>
        /// Date and time of last modification. Usually in UTC.
        /// </summary>
        /// <remarks>Best set via a <see cref="ITimeProvider"/>.</remarks>
        DateTime? DateModified { get; set; }

        /// <summary>
        /// Last one modifiying this instance.
        /// </summary>
        /// <remarks>Best set via a <see cref="ICurrentUserIdProvider"/>.</remarks>
       string ModifiedBy { get; set; }
    }
}