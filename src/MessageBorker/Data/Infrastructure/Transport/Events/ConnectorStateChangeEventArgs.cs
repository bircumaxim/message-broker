using System;
using Transport.Connectors;

namespace Transport.Events
{
    public delegate void ConnectorStateChangeHandler(object sender, ConnectorStateChangeEventArgs args);

    public class ConnectorStateChangeEventArgs : EventArgs
    {
        public ConnectionOrientedConnector ConnectionOrientedConnector { get; set; }
        public ConnectionState OldState { get; set; }
        public ConnectionState NewState { get; set; }

        public ConnectorStateChangeEventArgs(ConnectionOrientedConnector connectionOrientedConnector, ConnectionState oldState, ConnectionState newState)
        {
            ConnectionOrientedConnector = connectionOrientedConnector;
            OldState = oldState;
            NewState = newState;
        }
    }
}