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
            lock (_sendLock)
            {
                if (IsAlive)
                {
                    _logger.Error($"{GetType().Name} already started");
                    return;
                }
                StartCommunication();
                IsAlive = true;
            }
        }

        public override void Stop()
        {
            lock (_sendLock)
            {
                if (!IsAlive)
                {
                    _logger.Error($"{GetType().Name} already stoped");
                    return;
                }
                StopCommunication();
                _logger.Debug($"{GetType().Name} stoped");
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