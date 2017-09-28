using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serialization;
using Transport.Events;

namespace Transport
{
    public abstract class Connector :  IConnector
    {
        public static readonly HashSet<long> ConnectorIdsInUse = new HashSet<long>();
        public event MessageReceivedHandler MessageReceived;
        public long ConnectorId { get; }

        protected Connector(long connectorId)
        {
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
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(this, message));
        }

        #endregion
           
        private void Validate()
        {
            if (ConnectorIdsInUse.Contains(ConnectorId))
            {
                //TODO log here that such connector alrady exist
                throw new Exception("Connector id should be unique");
            }
        }
    }
}