using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Serialization;
using Transport.Events;

namespace Transport.Connectors
{
    public abstract class Connector : IConnector
    {
        private readonly ILog _logger;
        public static readonly HashSet<long> ConnectorIdsInUse = new HashSet<long>();
        public event MessageReceivedHandler MessageReceived;
        public long ConnectorId { get; }

        protected Connector(long connectorId)
        {
            _logger = LogManager.GetLogger(this.GetType());
            ConnectorId = connectorId;
            Validate();
        }

        #region Control IRum Methods

        public abstract void Start();

        public Task StartAsync()
        {
            return Task.Factory.StartNew(Start);
        }

        public abstract void Stop();

        #endregion

        #region Listeners

        protected void OnMessageReceived(Message message)
        {
            _logger.Info("New message was recevied");
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(this, message));
        }

        #endregion

        private void Validate()
        {
            if (ConnectorIdsInUse.Contains(ConnectorId))
            {
                _logger.Error("Connector id should be unique");
                throw new Exception("Connector id should be unique");
            }
        }
    }
}