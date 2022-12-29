using System;

namespace idee5.Common {
    /// <summary>
    /// Known values for the <see cref="Guid"/> Version field.
    /// </summary>
    public enum GuidVersion {
        /// <summary>
        /// Time-based (sequential) GUID.
        /// </summary>
        TimeBased = 1,

        /// <summary>
        /// DCE Security GUID with embedded POSIX UID/GID. See "DCE 1.1: Authentication and Security Services", Chapter 5 and "DCE 1.1: RPC", Appendix A.
        /// </summary>
        DCESecurity = 2,

        /// <summary>
        /// NativeName-based GUID using the MD5 hashing algorithm.
        /// </summary>
        NameBasedUsingMD5 = 3,

        /// <summary>
        /// Random GUID.
        /// </summary>
        Random = 4,

        /// <summary>
        /// NativeName-based GUID using the SHA-1 hashing algorithm.
        /// </summary>
        NameBasedUsingSHA1 = 5,
    }
}
