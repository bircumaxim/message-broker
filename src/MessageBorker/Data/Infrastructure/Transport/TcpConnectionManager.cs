using System;
using System.Threading.Tasks;
using Data;
using log4net;
using Serialization.WireProtocols;
using Transport.Connectors.Tcp;
using Transport.Tcp;
using Transport.Tcp.Events;

namespace Transport
{
    public class TcpConnectionManager : ConnectionManager
    {
        private readonly TcpConnectionListener _connectionListener;

        public TcpConnectionManager(int port)
        {
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
            //TODO add used wire protocol tu settings.
            var connectorId = Guid.NewGuid().GetHashCode();
            var tcpConnector = new TcpConnector(args.ClientSocket, connectorId, new DefaultWireProtocol());
            OnNewConnection(tcpConnector);
        }

        #endregion
    }
}