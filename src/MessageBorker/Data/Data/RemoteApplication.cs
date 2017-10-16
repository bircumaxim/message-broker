using System;
using Data.Events;
using log4net;
using Serialization;
using Transport.Connectors;
using Transport.Events;

namespace Data
{
    public class RemoteApplication
    {
        public string Name { get; }
        private readonly IConnector _connector;
        public event RemoteApplicationMessageReceived RemoteApplicationMessageReceived;
        
        public RemoteApplication(IConnector connector)
        {
            Name = connector.ConnectorId;
            _connector = connector;
            _connector.MessageReceived += OnMessageReceived;
            var logger = LogManager.GetLogger(GetType());
            Validate();
            logger.Info($"New remote application {_connector.GetType().Name} with id {Name}");
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            RemoteApplicationMessageReceived?.Invoke(this, new RemoteApplicationMessageReceivedEventArgs
            {
                Application = this,
                Message = args.Message
            });
        }

        public void Send(Message message)
        {
            _connector.SendMessage(message);
        }

        public void Stop()
        {
            _connector.MessageReceived -= OnMessageReceived;
            _connector.Stop();
        }
        
        private void Validate()
        {
            if (_connector != null) return;
            throw new NullReferenceException();
        }
    }
}