using System;
using System.Net;
using System.Threading.Tasks;

// http://blog.stephencleary.com/2013/01/async-oop-2-constructors.html
namespace idee5.Common.Net {
    public class AsyncSntpTimeProvider : ITimeProvider, IAsyncInitialization {
        public const string NtpPool = "pool.ntp.org";

        /// <summary>
        /// The cached correction offset.
        /// </summary>
        protected TimeSpan correctionOffset;

        /// <inheritdoc />
        public Task Initialization { get; }

        /// <summary>
        /// Gets the <see cref="NtpPacket.CorrectionOffset"/> from a ntp pool server and caches it for later use in <see cref="UtcNow"/>.
        /// This happens asynchronously, so ait for <see cref="Initialization"/> to complete before using <see cref="UtcNow"/>.
        /// </summary>
        public AsyncSntpTimeProvider() {
            Initialization = InitAsync();
        }

        /// <summary>
        /// Start getting the <see cref="NtpPacket.CorrectionOffset"/> from a ntp pool server.
        /// </summary>
        /// <returns></returns>
        private async Task InitAsync() {
            // use the first ntp server returned from the pool
            IPAddress address = (await Dns.GetHostEntryAsync(NtpPool).ConfigureAwait(false)).AddressList[0];
            using (var client = new NtpClient(address)) {
                correctionOffset = await client.GetCorrectionOffsetAsync().ConfigureAwait(false);
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