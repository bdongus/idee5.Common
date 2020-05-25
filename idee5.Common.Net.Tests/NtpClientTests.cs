using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Net.Tests {
    [TestClass]
    public class NtpClientTests
    {
        private const string _ntpAddress = "pool.ntp.org";

        [TestMethod]
        public void CanQueryValidRequest()
        {
            // Arrange
            NtpPacket result;

            // Act
            using (var client = new NtpClient(Dns.GetHostEntry(_ntpAddress).AddressList[0])) {
                result = client.Query();
            }

            // Assert
            Assert.IsNotNull(result.TransmitTimestamp);
        }

        [TestMethod]
        public async Task CanQueryValidRequestAsync()
        {
            // Arrange
            NtpPacket result;
            IPAddress address = (await Dns.GetHostEntryAsync(_ntpAddress).ConfigureAwait(false)).AddressList[0];

            // Act
            using (var client = new NtpClient(address)) {
                result = await client.QueryAsync().ConfigureAwait(false);
            }

            // Assert
            Assert.IsNotNull(result.TransmitTimestamp);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task CanCancelQueryAsync()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            IPAddress address = (await Dns.GetHostEntryAsync(_ntpAddress).ConfigureAwait(false)).AddressList[0];
            Task task;

            // Act
            using (var client = new NtpClient(address)) {
                task = Task.Factory.StartNew( () => client.QueryAsync(token).ConfigureAwait(false), token);
                cancellationTokenSource.Cancel(true);
                task.Wait(token);
            }
            // Assert
        }
    }
}
