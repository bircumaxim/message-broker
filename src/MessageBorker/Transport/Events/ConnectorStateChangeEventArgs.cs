using System;

namespace Transport.Events
{
    public delegate void ConnectorStateChangeHandler(object sender, ConnectorStateChangeEventArgs args);

    public class ConnectorStateChangeEventArgs : EventArgs
    {
        public Connector Connector { get; set; }
        public ConnectionState OldState { get; set; }
        public ConnectionState NewState { get; set; }

        public ConnectorStateChangeEventArgs(Connector connector, ConnectionState oldState, ConnectionState newState)
        {
            Connector = connector;
            OldState = oldState;
            NewState = newState;
        }
    }
}