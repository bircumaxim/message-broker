using System.Net;
using log4net;
using Serialization;

namespace Transport.Connectors
{
    public abstract class ConnectionLessConnector : Connector
    {
        private readonly ILog _logger;
        private readonly object _sendLock;
        protected bool IsAlive; 

        protected ConnectionLessConnector()
        {
            _logger = LogManager.GetLogger(GetType());
            _sendLock = new object();
            IsAlive = false;
        }
        
        #region Control IRun Methods

        public override void Start()
        {
            _logger.Info("Starting connection less comunication");
            lock (_sendLock)
            {
                if (IsAlive)
                {
                    _logger.Error("Connection less communication is already started");
                    return;
                }
                StartCommunication();
                IsAlive = true;
            }
        }

        public override void Stop()
        {
            _logger.Info("Connector is going to be disconnected"); 
            lock (_sendLock)
            {
                if (!IsAlive)
                {
                    _logger.Error("Connectiop less comunication is already stoped");
                    return;
                }
                StopCommunication();
                IsAlive = false;
            }
        }

        #endregion

        protected abstract void StartCommunication();

        protected abstract void StopCommunication();
        
        public override void SendMessage(Message message)
        {
            lock (_sendLock)
            {
                SendMessageInternal(message);
            }
        }
        
        protected abstract void SendMessageInternal(Message message);
    }
}