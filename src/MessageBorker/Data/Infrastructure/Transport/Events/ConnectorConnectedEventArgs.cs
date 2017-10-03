using Transport.Connectors;

namespace Transport.Events
{
    public delegate void ConnectorConnectedHandler(object sender, ConnectorConnectedEventArgs args);

    public class ConnectorConnectedEventArgs
    {
        public IConnector Connector { get; set; }

        public ConnectorConnectedEventArgs(IConnector connector)
        {
            Connector = connector;
        }
    }
}