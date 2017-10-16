using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Messages.Udp;
using Serialization;
using Serialization.Deserializer;
using Serialization.WireProtocol;
using Transport.Connectors.Udp.Events;

namespace Transport.Connectors.Udp
{
    public class UdpReceiver : IRun
    {
        private readonly ILog _logger;
        private readonly int _port;
        public Socket Socket { get; }
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ManualResetEvent _allDone;
        private bool _isAlive;
        public event UdpMessageReceivedHandler UdpMessageReceived;
        private readonly IWireProtocol _wireProtocol;

        public UdpReceiver(int port, IWireProtocol wireProtocol)
        {
            _port = port;
            _isAlive = false;
            _wireProtocol = wireProtocol;
            _logger = LogManager.GetLogger(GetType());
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _allDone = new ManualResetEvent(false);
            _cancellationTokenSource = new CancellationTokenSource();
            Validate();
        }

        #region IRun controll methods

        public void Start()
        {
            _logger.Debug($"Starting {GetType().Name}");
            if (_isAlive)
            {
                _logger.Error($"{GetType().Name} is already started");
                return;
            }
            BindSocketToEndpoint(new IPEndPoint(IPAddress.Any, _port));
            _isAlive = true;
            _logger.Info($"Started Socket protocol=\"Udp\" port=\"{_port}\"");
            StartReceivingMessages();
        }

        public Task StartAsync()
        {
            return Task.Factory.StartNew(Start, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _logger.Debug($"Stoping {GetType().Name}");
            if (!_isAlive)
            {
                _logger.Error($"{GetType().Name} stoped already");
                return;
            }
            _isAlive = false;
            _cancellationTokenSource.Cancel();
            Socket.Close();
            Socket.Dispose();
        }

        #endregion

        private void BindSocketToEndpoint(EndPoint endPoint)
        {
            try
            {
                Socket.Bind(endPoint);
            }
            catch (Exception ex)
            {
                _logger.Error($"{GetType().Name} Socket binding exception");
                throw;
            }
        }

        private void StartReceivingMessages()
        {
            while (_isAlive)
            {
                try
                {
                    _allDone.Reset();
                    var state = new StateObject {Buffer = new byte[Socket.SendBufferSize]};
                    Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                _allDone.WaitOne();
            }
        }

        public void ReadCallback(IAsyncResult result)
        {
            if (!_isAlive) return;
            _allDone.Set();
            if (Socket == null) return;
            int bytesRead = 0;
            try
            {
                bytesRead = Socket.EndReceive(result);
            }
            catch (Exception ex)
            {
                // ignored
            }
            if (bytesRead > 0)
            {
                var state = result.AsyncState as StateObject;
                if (state == null) return;
                var message =
                    _wireProtocol.ReadMessage(new DefaultDeserializer(new MemoryStream(state.Buffer, 0, bytesRead)));
                if (message != null && message.MessageTypeName == typeof(UdpMessageWrapper).Name)
                {
                    OnMessageReceived(message as UdpMessageWrapper);
                }
            }
        }

        protected void OnMessageReceived(UdpMessageWrapper udpMessageWrapper)
        {
            var message =
                _wireProtocol.ReadMessage(new DefaultDeserializer(new MemoryStream(udpMessageWrapper.Message)));
            UdpMessageReceived?.Invoke(this, new UdpMessageReceivedEventArgs(udpMessageWrapper.ClientName, message));
        }

        private bool IsPortAvailable(int port)
        {
            return IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .Any(tcpConnectionInformation => tcpConnectionInformation.LocalEndPoint.Port != port);
        }

        private void Validate()
        {
            if (!IsPortAvailable(_port) || Socket == null || _allDone == null || _wireProtocol == null)
            {
                _logger.Error($"{GetType().Name} validation failed");
                throw new NullReferenceException();
            }
        }
    }

    internal class StateObject
    {
        public byte[] Buffer { get; set; }
    }
}