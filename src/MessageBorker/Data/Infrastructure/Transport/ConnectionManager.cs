using System.Threading.Tasks;
using Transport;
using Transport.Connectors;
using Transport.Events;

namespace Data
{
    public abstract class ConnectionManager : IConnectionManager
    {   
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