using System;
using System.Threading.Tasks;
using Serialization.WireProtocols;
using Transport.Tcp;
using Transport.Tcp.Events;

namespace Data
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
            try
            {
                //TODO add used wire protocol tu settings.
                var tcpConnector = new TcpConnector(args.ClientSocket, Guid.NewGuid().GetHashCode(), new DefaultWireProtocol());
                OnNewConnection(tcpConnector);
            }
            catch (Exception ex)
            {
                //TODO log here exception.
            }
        }

        #endregion
    }
}