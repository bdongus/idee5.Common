using System;

namespace idee5.Common {
    /// <summary>
    /// Time provider interface.
    /// </summary>
    public interface ITimeProvider {
        /// <summary>
        /// Returns the current instant in UTC.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Returns current instant in UTC as <see cref="DateTimeOffset"/>.
        /// </summary>
        DateTimeOffset UtcNowOffset { get; }
    }
}