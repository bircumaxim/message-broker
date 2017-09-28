using System.Net;
using Serialization;

namespace Transport
{
    public abstract class ConnectionLessConnector : Connector, IConnectionLessConnector
    {
        private readonly object _sendLock;
        protected bool IsAlive; 

        protected ConnectionLessConnector(long connectorId) : base(connectorId)
        {
            _sendLock = new object();
            IsAlive = false;
        }
        
        #region Control IRun Methods

        public override void Start()
        {
            //TODO log here that communication is going to be started
            lock (_sendLock)
            {
                if (IsAlive)
                {
                    //TODO communication is already started
                    return;
                }
                StartCommunication();
                IsAlive = true;
            }
        }

        public override void Stop()
        {
            //TODO log here that connector is going to be disconnected
            lock (_sendLock)
            {
                if (!IsAlive)
                {
                    //TODO communication is already stoped
                    return;
                }
                StopCommunication();
                IsAlive = false;
            }
        }

        #endregion

        protected abstract void StartCommunication();

        protected abstract void StopCommunication();
        
        public void SendMessage(Message message, EndPoint endPoint)
        {
            lock (_sendLock)
            {
                SendMessageInternal(message, endPoint);
            }
        }
        
        protected abstract void SendMessageInternal(Message message, EndPoint endPoint);
    }
}