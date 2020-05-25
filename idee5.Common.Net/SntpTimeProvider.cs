using System;
using System.Net;

namespace idee5.Common.Net {
    public class SntpTimeProvider : ITimeProvider {
        public const string NtpPool = "pool.ntp.org";

        /// <summary>
        /// The cached correction offset.
        /// </summary>
        protected TimeSpan correctionOffset;

        /// <summary>
        /// Gets the <see cref="NtpPacket.CorrectionOffset"/> from a ntp pool server and caches it for later use in <see cref="UtcNow"/>.
        /// </summary>
        public SntpTimeProvider() {
            using (var client = new NtpClient(Dns.GetHostEntry(NtpPool).AddressList[0])) {
                correctionOffset = client.GetCorrectionOffset();
            }
        }

        /// <summary>
        /// Returns the current UTC time corrected by the cached offset.
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow + correctionOffset;

        /// <inheritdoc />
        public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow + correctionOffset;
    }
}