using System;

namespace idee5.Common.Net {
    /// <summary>
    /// Represents errors that occur in SNTP packets or during SNTP operation.
    /// </summary>
    public class NtpException : Exception {
        /// <summary>
        /// Gets the SNTP packet that caused this exception, if any.
        /// </summary>
        /// <value>
        /// SNTP packet that caused this exception, usually reply packet,
        /// or <c>null</c> if the error is not specific to any packet.
        /// </value>
        public NtpPacket Packet { get; }

        internal NtpException(NtpPacket packet, string message)
            : base(message)
        {
            Packet = packet;
        }

        /// <inheritdoc />
        public NtpException() : base()
        {
        }

        /// <inheritdoc />
        protected NtpException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public NtpException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public NtpException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}