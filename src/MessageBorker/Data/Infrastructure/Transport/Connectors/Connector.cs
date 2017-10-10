using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using log4net;
using Serialization;
using Transport.Events;

namespace Transport.Connectors
{
    public abstract class Connector : IConnector
    {
        private readonly ILog _logger;
        public static HashSet<string> ConnectorIdsInUse = new HashSet<string>();
        public event MessageReceivedHandler MessageReceived;
        public string ConnectorId { get; }

        protected Connector()
        {
            _logger = LogManager.GetLogger(GetType());
            ConnectorId = Guid.NewGuid().ToString();
            Validate();
        }

        #region Control IRum Methods

        public abstract void Start();

        public Task StartAsync()
        {
            return Task.Factory.StartNew(Start);
        }

        public abstract void Stop();

        public abstract void SendMessage(Message message);

        #endregion

        #region Listeners

        protected void OnMessageReceived(Message message)
        {
            _logger.Info($"Received message=\"{message.MessageTypeName}\" by {GetType().Name} with id {ConnectorId}");
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
            ConnectorIdsInUse.Add(ConnectorId);
        }
    }
}