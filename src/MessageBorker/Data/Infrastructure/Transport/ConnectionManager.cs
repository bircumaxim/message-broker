using System.Threading.Tasks;
using Serialization;
using Serialization.WireProtocol;
using Transport.Connectors;
using Transport.Events;

namespace Transport
{
    public abstract class ConnectionManager : IConnectionManager
    {
        protected IWireProtocol WireProtocol;

        protected ConnectionManager(IWireProtocol wireProtocol)
        {
            WireProtocol = wireProtocol;
        }

        public event ConnectorConnectedHandler ConnectorConnected;

        public abstract void Start();

        public abstract Task StartAsync();

        public abstract void Stop();

        protected void OnNewConnection(IConnector connector)
        {
            ConnectorConnected?.Invoke(this, new ConnectorConnectedEventArgs(connector));
        }
    }
}