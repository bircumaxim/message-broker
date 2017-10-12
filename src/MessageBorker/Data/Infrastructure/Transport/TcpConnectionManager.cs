using System.Threading.Tasks;
using Serialization;
using Serialization.WireProtocol;
using Transport.Connectors.Tcp;
using Transport.Connectors.Tcp.Events;

namespace Transport
{
    public class TcpConnectionManager : ConnectionManager
    {
        private readonly int _maxMessageLength;
        private readonly TcpConnectionListener _connectionListener;

        public TcpConnectionManager(int port, IWireProtocol wireProtocol, int maxMessageLength) :
            base(wireProtocol)
        {
            _maxMessageLength = maxMessageLength;
            _connectionListener = new TcpConnectionListener(port);
            _connectionListener.TcpClientConnected += OnTcpClientConnected;
        }

        #region Controll IRun methods

        public override void Start()
        {
            _connectionListener.Start();
        }

        public override Task StartAsync()
        {
            return _connectionListener.StartAsync();
        }

        public override void Stop()
        {
            _connectionListener.Stop();
        }

        #endregion

        #region Listeners

        private void OnTcpClientConnected(object sender, TcpClientConnectedEventArgs args)
        {
            var tcpConnector = new TcpConnector(args.ClientSocket, WireProtocol, _maxMessageLength);
            OnNewConnection(tcpConnector);
        }

        #endregion
    }
}