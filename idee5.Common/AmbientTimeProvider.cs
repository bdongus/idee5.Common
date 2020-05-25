using System;

namespace idee5.Common {
    /// <summary>
    /// <see cref="ITimeProvider"/> wrapped in an ambient context.
    /// </summary>
    /// <example>public readonly AmbientTimeProvider TimeProvider = new AmbientTimeProvider();</example>
    public class AmbientTimeProvider : AmbientService<ITimeProvider>, ITimeProvider
    {
        /// <inheritdoc />
        protected override ITimeProvider DefaultCreate() => new DefaultTimeProvider();

        /// <inheritdoc />
        public DateTime UtcNow => Instance.UtcNow;

        /// <inheritdoc />
        public DateTimeOffset UtcNowOffset => Instance.UtcNowOffset;
    }
}