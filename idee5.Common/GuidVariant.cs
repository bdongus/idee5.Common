using System;

namespace idee5.Common {
    /// <summary>
    /// Known values for the <see cref="Guid"/> Variant field.
    /// </summary>
    public enum GuidVariant {
        /// <summary>
        /// Reserved for NCS backward compatibility.
        /// </summary>
        NCSBackwardCompatibility = 0,

        /// <summary>
        /// A GUID conforming to RFC 4122.
        /// </summary>
        RFC4122 = 4,

        /// <summary>
        /// Reserved for Microsoft backward compatibility.
        /// </summary>
        MicrosoftBackwardCompatibility = 6,

        /// <summary>
        /// Reserved for future definition.
        /// </summary>
        ReservedForFutureDefinition = 7,
    }
}
