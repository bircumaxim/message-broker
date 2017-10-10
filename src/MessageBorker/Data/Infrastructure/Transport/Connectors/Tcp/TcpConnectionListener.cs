using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Transport.Connectors.Tcp.Events;

namespace Transport.Connectors.Tcp
{
    public class TcpConnectionListener : IRun
    {
        private readonly ILog _logger;
        private readonly int _port;
        private readonly Socket _listenerSocket;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ManualResetEvent _allDone;
        private bool _isAlive;
        public event TcpClientConnectedHandler TcpClientConnected;

        public TcpConnectionListener(int port)
        {
            _logger = LogManager.GetLogger(GetType());
            _port = port;
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
            _isAlive = !_isAlive;
            _logger.Info($"Started Socket protocol=\"Tcp\" port=\"{_port}\"");
            TcpSocketListening();
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
            _isAlive = !_isAlive;
            _cancellationTokenSource.Cancel();
            _listenerSocket.Close();
            _listenerSocket.Dispose();
        }

        #endregion

        private void BindSocketToEndpoint(EndPoint endPoint)
        {
            try
            {
                _listenerSocket.Bind(endPoint);
                _listenerSocket.Listen(0);
            }
            catch (Exception ex)
            {
                _logger.Error($"{GetType().Name} Socket binding exception");
                throw;
            }
        }

        private void TcpSocketListening()
        {
            _logger.Info("Waiting for connections...");
            while (_isAlive)
            {
                _allDone.Reset();
                _listenerSocket.BeginAccept(OnSocketAccepted, _listenerSocket);
                _allDone.WaitOne();
            }
        }

        private void OnSocketAccepted(IAsyncResult result)
        {
            if (!_isAlive) return;
            _allDone.Set();
            var socketListener = (Socket) result.AsyncState;
            if (socketListener == null) return;
            var accptedSocket = socketListener.EndAccept(result);
            _logger.Debug($"Socket accepted protocol=\"{accptedSocket.ProtocolType}\" endpoint=\"{accptedSocket.RemoteEndPoint}\"");
            TcpClientConnected?.Invoke(this, new TcpClientConnectedEventArgs {ClientSocket = accptedSocket});
        }

        private bool IsPortAvailable(int port)
        {
            return IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .Any(tcpConnectionInformation => tcpConnectionInformation.LocalEndPoint.Port != port);
        }
        
        private void Validate()
        {
            if (!IsPortAvailable(_port) || _listenerSocket == null || _allDone == null)
            {
                _logger.Error("Entity validation error");
                throw new Exception("Given port is not available");
            }
        }
    }
}