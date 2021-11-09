using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Net {
    // Taken from GuerrillaNtp: https://guerrillantp.machinezoo.com
    /// <summary>
    /// Represents UDP socket used to communicate with RFC4330-compliant SNTP/NTP server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// See <a href="https://guerrillantp.machinezoo.com/">project homepage</a> for guidance on how to use GuerrillaNtp.
    /// Most applications should just call <see cref="GetCorrectionOffset" />
    /// after instantiating this class. Method <see cref="NtpClient.Query" />
    /// can be used to obtain additional details stored in reply <see cref="NtpPacket" />.
    /// </para>
    /// <para>
    /// This class holds unmanaged resources (the socket) and callers are responsible
    /// for calling <see cref="Dispose" /> when they are done,
    /// perhaps by instantiating this class in <c>using</c> block.
    /// </para>
    /// <para>
    /// It is application responsibility to be a good netizen,
    /// which most importantly means using reasonable polling intervals
    /// and exponential backoff when querying public NTP server.
    /// </para>
    /// </remarks>
    public class NtpClient : IDisposable {
        private readonly Socket _socket;
        private readonly IPEndPoint _endpoint;

        /// <summary>
        /// Gets or sets the timeout for SNTP queries.
        /// </summary>
        /// <value>
        /// Timeout for SNTP queries. Default is one second.
        /// </value>
        public TimeSpan Timeout {
            get { return TimeSpan.FromMilliseconds(_socket.ReceiveTimeout); }
            set {
                if (value < TimeSpan.FromMilliseconds(1))
                    throw new ArgumentOutOfRangeException();
                _socket.ReceiveTimeout = Convert.ToInt32(value.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Creates new <see cref="NtpClient" /> from server endpoint.
        /// </summary>
        /// <param name="endpoint">Endpoint of the remote SNTP server.</param>
        /// <seealso cref="NtpClient.#ctor(System.Net.IPAddress,System.Int32)" />
        /// <seealso cref="Dispose" />
        public NtpClient(IPEndPoint endpoint)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            _socket = new Socket(_endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = 1000
            };
        }

        /// <summary>
        /// Establish connection to the (s)ntp host.
        /// </summary>
        /// <param name="endpoint">IP endpoint of the (s)ntp host.</param>
        /// <exception cref="SocketException"></exception>
        private void ConnectSocket(IPEndPoint endpoint)
        {
            try {
                _socket.Connect(endpoint);
            }
            catch (SocketException) {
                _socket.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Establish a connection to the (s)ntp host asynchronously.
        /// </summary>
        /// <param name="endpoint">IP endpoint of the (s)ntp host.</param>
        /// <exception cref="SocketException"></exception>
        private async Task ConnectSocketAsync(IPEndPoint endpoint)
        {
            try {
                await _socket.ConnectAsync(endpoint).ConfigureAwait(false);
            }
            catch (SocketException) {
                _socket.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Creates new <see cref="NtpClient" /> from server's IP address and optional port.
        /// </summary>
        /// <param name="address">IP address of remote SNTP server</param>
        /// <param name="port">Port of remote SNTP server. Default is 123 (standard NTP port).</param>
        /// <seealso cref="NtpClient.#ctor(System.Net.IPEndPoint)" />
        /// <seealso cref="Dispose" />
        public NtpClient(IPAddress address, int port = 123) : this(new IPEndPoint(address, port)) { }

        /// <summary>
        /// Releases all resources held by <see cref="NtpClient" />.
        /// </summary>
        /// <remarks>
        /// <see cref="NtpClient" /> holds reference to <see cref="Socket" />,
        /// which must be explicitly released to avoid memory leaks.
        /// </remarks>
        public void Dispose() { _socket.Dispose(); }

        /// <summary>
        /// Queries the SNTP server and returns correction offset.
        /// </summary>
        /// <remarks>
        /// Use this method if you just want correction offset from the server.
        /// Call <see cref="NtpClient.Query" /> to obtain <see cref="NtpPacket" />
        /// with additional information besides <see cref="NtpPacket.CorrectionOffset" />.
        /// </remarks>
        /// <returns>
        /// Offset that should be added to local time to match server time.
        /// </returns>
        /// <exception cref="NtpException">Thrown when the server responds with invalid reply packet.</exception>
        /// <exception cref="SocketException">
        /// Thrown when no reply is received before <see cref="Timeout" /> is reached
        /// or when there is an error communicating with the server.
        /// </exception>
        /// <seealso cref="Query" />
        /// <seealso cref="NtpPacket.CorrectionOffset" />
        public TimeSpan GetCorrectionOffset() { return Query().CorrectionOffset; }

        /// <summary>
        /// Queries the SNTP server asynchronously and returns correction offset.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// Use this method if you just want correction offset from the server.
        /// Call <see cref="NtpClient.Query" /> to obtain <see cref="NtpPacket" />
        /// with additional information besides <see cref="NtpPacket.CorrectionOffset" />.
        /// </remarks>
        /// <returns>
        /// Offset that should be added to local time to match server time.
        /// </returns>
        /// <exception cref="NtpException">Thrown when the server responds with invalid reply packet.</exception>
        /// <exception cref="SocketException">
        /// Thrown when no reply is received before <see cref="Timeout" /> is reached
        /// or when there is an error communicating with the server.
        /// </exception>
        /// <seealso cref="Query" />
        /// <seealso cref="NtpPacket.CorrectionOffset" />
        public async Task<TimeSpan> GetCorrectionOffsetAsync(CancellationToken cancellationToken = default) {
            return (await QueryAsync(cancellationToken).ConfigureAwait(false)).CorrectionOffset;
        }

        /// <summary>
        /// Queries the SNTP server with configurable <see cref="NtpPacket" /> request.
        /// </summary>
        /// <param name="request">SNTP request packet to use when querying the network time server.</param>
        /// <returns>SNTP reply packet returned by the server.</returns>
        /// <remarks>
        /// <see cref="NtpPacket.#ctor" /> constructor
        /// creates valid request packet, which you can further customize.
        /// If you don't need any customization of the request packet, call <see cref="NtpClient.Query" /> instead.
        /// Returned <see cref="NtpPacket" /> contains correction offset in
        /// <see cref="NtpPacket.CorrectionOffset" /> property.
        /// </remarks>
        /// <exception cref="NtpException">
        /// Thrown when the request packet is invalid or when the server responds with invalid reply packet.
        /// </exception>
        /// <exception cref="SocketException">
        /// Thrown when no reply is received before <see cref="Timeout" /> is reached
        /// or when there is an error communicating with the server.
        /// </exception>
        /// <seealso cref="GetCorrectionOffset" />
        /// <seealso cref="NtpPacket.CorrectionOffset" />
        /// <seealso cref="Query" />
        public NtpPacket Query(NtpPacket request)
        {
            request.ValidateRequest();
            ConnectSocket(_endpoint);
            _socket.Send(request.Bytes);
            byte[] response = new byte[160];
            int received = _socket.Receive(response);
            byte[] truncated = new byte[received];
            Array.Copy(response, truncated, received);
            var reply = new NtpPacket(truncated) { DestinationTimestamp = DateTime.UtcNow };
            reply.ValidateReply(request);
            return reply;
        }

        /// <summary>
        /// Async querying the SNTP server with configurable <see cref="NtpPacket" /> request.
        /// </summary>
        /// <param name="request">SNTP request packet to use when querying the network time server.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>SNTP reply packet returned by the server.</returns>
        /// <remarks>
        /// <see cref="NtpPacket.#ctor" /> constructor
        /// creates valid request packet, which you can further customize.
        /// If you don't need any customization of the request packet, call <see cref="NtpClient.Query" /> instead.
        /// Returned <see cref="NtpPacket" /> contains correction offset in
        /// <see cref="NtpPacket.CorrectionOffset" /> property.
        /// </remarks>
        /// <exception cref="NtpException">
        /// Thrown when the request packet is invalid or when the server responds with invalid reply packet.
        /// </exception>
        /// <exception cref="SocketException">
        /// Thrown when no reply is received before <see cref="Timeout" /> is reached
        /// or when there is an error communicating with the server.
        /// </exception>
        /// <seealso cref="GetCorrectionOffset" />
        /// <seealso cref="NtpPacket.CorrectionOffset" />
        /// <seealso cref="Query" />
        public async Task<NtpPacket> QueryAsync(NtpPacket request, CancellationToken cancellationToken = default)
        {
            request.ValidateRequest();
            cancellationToken.ThrowIfCancellationRequested();
            await ConnectSocketAsync(_endpoint).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            // visual studio hides the extensions as they are not available in .net 4.6.1: https://github.com/dotnet/standard/issues/466
            await _socket.SendAsync(new ArraySegment<byte>(request.Bytes), SocketFlags.None).ConfigureAwait(false);
            var response = new ArraySegment<byte>(new byte[160]);
            int received = await _socket.ReceiveAsync(response, SocketFlags.None).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            byte[] truncated = new byte[received];
            Array.Copy(response.Array, truncated, received);
            var reply = new NtpPacket(truncated) { DestinationTimestamp = DateTime.UtcNow };
            reply.ValidateReply(request);
            return reply;
        }

        /// <summary>
        /// Queries the SNTP server with default options.
        /// </summary>
        /// <remarks>
        /// Use this method to obtain additional details from the returned <see cref="NtpPacket" />
        /// besides <see cref="NtpPacket.CorrectionOffset" />.
        /// If you just need the correction offset, call <see cref="GetCorrectionOffset" /> instead.
        /// You can customize request packed by calling <see cref="Query(NtpPacket)" />.
        /// </remarks>
        /// <returns>SNTP reply packet returned by the server.</returns>
        /// <exception cref="NtpException">Thrown when the server responds with invalid reply packet.</exception>
        /// <exception cref="SocketException">
        /// Thrown when no reply is received before <see cref="Timeout" /> is reached
        /// or when there is an error communicating with the server.
        /// </exception>
        /// <seealso cref="GetCorrectionOffset" />
        /// <seealso cref="NtpPacket.CorrectionOffset" />
        /// <seealso cref="Query(NtpPacket)" />
        public NtpPacket Query() { return Query(new NtpPacket()); }

        /// <summary>
        /// Queries the SNTP server asynchronously with default options.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// Use this method to obtain additional details from the returned <see cref="NtpPacket" />
        /// besides <see cref="NtpPacket.CorrectionOffset" />.
        /// If you just need the correction offset, call <see cref="GetCorrectionOffset" /> instead.
        /// You can customize request packed by calling <see cref="Query(NtpPacket)" />.
        /// </remarks>
        /// <returns>SNTP reply packet returned by the server.</returns>
        /// <exception cref="NtpException">Thrown when the server responds with invalid reply packet.</exception>
        /// <exception cref="SocketException">
        /// Thrown when no reply is received before <see cref="Timeout" /> is reached
        /// or when there is an error communicating with the server.
        /// </exception>
        /// <seealso cref="GetCorrectionOffset" />
        /// <seealso cref="NtpPacket.CorrectionOffset" />
        /// <seealso cref="Query(NtpPacket)" />
        public Task<NtpPacket> QueryAsync(CancellationToken cancellationToken = default) { return QueryAsync(new NtpPacket(), cancellationToken); }
    }
}