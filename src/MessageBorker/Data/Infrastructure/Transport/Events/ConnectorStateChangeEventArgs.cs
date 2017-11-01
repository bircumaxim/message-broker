using System;
using Transport.Connectors;

namespace Transport.Events
{
    public delegate void ConnectorStateChangeHandler(object sender, ConnectorStateChangeEventArgs args);

    public class ConnectorStateChangeEventArgs : EventArgs
    {
        public IConnector Connector { get; set; }
        public ConnectionState OldState { get; set; }
        public ConnectionState NewState { get; set; }

        public ConnectorStateChangeEventArgs(IConnector connector, ConnectionState oldState, ConnectionState newState)
        {
            Connector = connector;
            OldState = oldState;
            NewState = newState;
        }
    }
}