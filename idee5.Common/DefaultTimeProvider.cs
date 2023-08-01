using System;

namespace idee5.Common {
    /// <inheritdoc />
    public class DefaultTimeProvider : ITimeProvider {
        /// <inheritdoc />
        public DateTime UtcNow => DateTime.UtcNow;

        /// <inheritdoc />
        public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
    }
}