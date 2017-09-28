using System;
using System.Threading.Tasks;
using Serialization;
using Transport.Events;

namespace Transport
{
    public abstract class Connector : IConnector
    {
        private readonly object _sendLock;
        public long ConnectorId { get; }
        public event ConnectorStateChangeHandler StateChanged;
        public event MessageReceivedHandler MessageReceived;
        public ConnectionState ConnectionState { get; set; }
        public CommunicationWay CommunicationWay { get; set; }

        protected Connector(long connectorId)
        {
            ConnectorId = connectorId;
            CommunicationWay = CommunicationWay.Send;
            ConnectionState = ConnectionState.Disconnected;
            _sendLock = new object();
        }

        #region Control IRun Methods

        public void Start()
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
                StartCommunicaiton();
                OnStateChange(ConnectionState.Connected);
            }
        }

        public Task StartAsync()
        {
            return Task.Factory.StartNew(Start);
        }

        public void Stop()
        {
            //TODO log here that connector is going to be disconnected
            lock (_sendLock)
            {
                OnStateChange(ConnectionState.Disconnecting);
                StopCommunicaiton();
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

        protected void OnMessageReceived(Message message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(this, message));
        }

        #endregion

        public void SendMessage(Message message)
        {
            lock (_sendLock)
            {
                SendMessageInternal(message);                
            }
        }
        
        protected abstract void StartCommunicaiton();

        protected abstract void StopCommunicaiton();

        protected abstract void SendMessageInternal(Message message);

    }
}