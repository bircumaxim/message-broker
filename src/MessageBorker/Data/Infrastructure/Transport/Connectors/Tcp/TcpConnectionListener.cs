using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Transport.Tcp.Events;

namespace Transport.Connectors.Tcp
{
    public class TcpConnectionListener : IRun
    {
        private readonly ILog _logger;
        private readonly int _port;
        private readonly Socket _listenerSocket;
        private readonly ManualResetEvent _allDone;
        public bool IsListening { get; set; }
        public event TcpClientConnectedHandler TcpClientConnected;

        public TcpConnectionListener(int port)
        {
            _logger = LogManager.GetLogger(GetType());
            _port = port;
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _allDone = new ManualResetEvent(false);
            Validate();
        }

        #region IRun controll methods

        public void Start()
        {
            _logger.Info("Starting listener socket");
            if (IsListening)
            {
                _logger.Error("Socket already listening");
                return;
            }
            BindSocketToEndpoint(new IPEndPoint(0, _port));
            IsListening = !IsListening;
            _logger.Debug($"Socket was started on port {_port}");
            TcpSocketListening();
        }

        public Task StartAsync()
        {
            return Task.Factory.StartNew(Start);
        }

        public void Stop()
        {
            if (_listenerSocket == null)
            {
                Console.WriteLine("Huinea");
            }
            _logger.Info("Stoping listener socket");
            if (!IsListening)
            {
                _logger.Error("Socket is not listening already");
                return;
            }
            IsListening = !IsListening;
//            _listenerSocket.Dispose();
        }

        #endregion

        private void BindSocketToEndpoint(EndPoint endPoint)
        {
            try
            {
                _listenerSocket.Bind(endPoint);
                _listenerSocket.Listen(0);
            }
            catch (Exception e)
            {
                _logger.Error("Socket binding exception");
                Console.WriteLine(e);
                throw;
            }
        }

        private void TcpSocketListening()
        {
            _logger.Info("Waiting for connections");
            while (IsListening)
            {
                _allDone.Reset();
                _listenerSocket.BeginAccept(OnSocketAccepted, _listenerSocket);
                _allDone.WaitOne();
            }
        }

        private void OnSocketAccepted(IAsyncResult result)
        {
            _allDone.Set();
            var socketListener = (Socket) result.AsyncState;
            var accptedSocket = socketListener.EndAccept(result);
            _logger.Info("New client socket was accepted");
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
                //TODO check why this is not working correctly
                //throw new Exception("Given port is not available");
            }
        }
    }
}