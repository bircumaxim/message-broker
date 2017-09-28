using System;
using Serialization;
using Transport.Events;

namespace Transport
{
    public abstract class ConnectionOrientedConnector : Connector, IConnectionOrientedConnector
    {
        private readonly object _sendLock;
        public event ConnectorStateChangeHandler StateChanged;
        public ConnectionState ConnectionState { get; set; }
        public CommunicationWay CommunicationWay { get; set; }

        protected ConnectionOrientedConnector(long connectorId) : base(connectorId)
        {
            CommunicationWay = CommunicationWay.Send;
            ConnectionState = ConnectionState.Disconnected;
            _sendLock = new object();
        }

        #region Control IRun Methods

        public override void Start()
        {
            //TODO log here that connector is starting
            if (ConnectionState != ConnectionState.Disconnected)
            {
                //TODO log here exception
                throw new Exception("Communicator is already connected");
            }

            lock (_sendLock)
            {
                OnStateChange(ConnectionState.Connecting);
                StartCommunication();
                OnStateChange(ConnectionState.Connected);
            }
        }

        public override void Stop()
        {
            //TODO log here that connector is going to be disconnected
            lock (_sendLock)
            {
                OnStateChange(ConnectionState.Disconnecting);
                StopCommunication();
                OnStateChange(ConnectionState.Disconnected);
            }
        }

        #endregion

        #region Listeners

        protected void OnStateChange(ConnectionState connectionState)
        {
            ConnectionState = connectionState;
            StateChanged?.Invoke(this, new ConnectorStateChangeEventArgs(this, connectionState, ConnectionState));
        }

        #endregion

        protected abstract void StartCommunication();

        protected abstract void StopCommunication();
        
        public void SendMessage(Message message)
        {
            lock (_sendLock)
            {
                SendMessageInternal(message);                
            }
        }

        protected abstract void SendMessageInternal(Message message);
    }
}