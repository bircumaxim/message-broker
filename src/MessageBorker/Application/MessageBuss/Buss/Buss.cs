using System;
using System.IO;
using MessageBuss.Brocker;
using MessageBuss.Brocker.Events;
using MessageBuss.Buss.Events;
using Messages;
using Messages.Payload;
using Messages.ServerInfo;
using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace MessageBuss.Buss
{
    public class Buss
    {
        private readonly BrockerClient _brocker;
        public event MessageReceivedHandler MessageReceived;

        public Buss(BrockerClient brocker)
        {
            _brocker = brocker;
            _brocker.MessageReceivedFromBrockerHandler += OnMessageReceivedFromBrocker;
        }

        #region Buss features

        public void Ping()
        {
            _brocker.Ping();
        }

        public void RequestServerInfo()
        {
            _brocker.SendOrEnqueue(new ServerGerneralInfoRequest());
        }
        
        public void Request(string queueName)
        {
            _brocker.SendOrEnqueue(new PayloadRequest {QueueName = queueName});
        }

        public void Publish(string exchangeName, string routingKey, Message payload, bool isDurable = false)
        {
            _brocker.SendOrEnqueue(CreateDefaultMessage(exchangeName, routingKey, payload, isDurable));
        }

        public void Fanout(Message payload, bool isDurable = false)
        {
            Publish(GetExchangeNameForType("Fanout"), "", payload, isDurable);
        }

        public void Direct(Message payload, string routingKey, bool isDurable = false)
        {
            Publish(GetExchangeNameForType("Direct"), routingKey, payload, isDurable);
        }

        public void Topic(Message payload, string routingKey, bool isDurable = false)
        {
            Publish(GetExchangeNameForType("Topic"), routingKey, payload, isDurable);
        }

        public void Dispose()
        {
            _brocker.MessageReceivedFromBrockerHandler -= OnMessageReceivedFromBrocker;
            _brocker.Stop();
        }

        #endregion

        private void OnMessageReceivedFromBrocker(object sender, BrockerClientMessageReceivedEventArgs args)
        {
            switch (args.Message.MessageTypeName)
            {
                case "DefaultMessageResponse":
                    var message = args.Message as DefaultMessageResponse;
                    Message payload = null;
                    if (message != null)
                    {
                        var deserializer = new DefaultDeserializer(new MemoryStream(message.Payload));
                        payload = _brocker.WireProtocol.ReadMessage(deserializer);
                    }
                    MessageReceived?.Invoke(this, new MessegeReceviedEventArgs(payload));
                    break;
                default:
                    MessageReceived?.Invoke(this, new MessegeReceviedEventArgs(args.Message));
                    break;
            }
        }

        private string GetExchangeNameForType(string exchangeType)
        {
            string exchangeName;
            _brocker.DefautlExchanges.TryGetValue(exchangeType, out exchangeName);
            if (exchangeName == null)
            {
                throw new Exception($"Default exchange for {exchangeType} was not set !!!");
            }
            return exchangeName;
        }

        private Message CreateDefaultMessage(string exchangeName, string routingKey, Message payload,
            bool isDurable = false)
        {
            var defaultMessage = new PayloadMessage
            {
                IsDurable = isDurable,
                RoutingKey = routingKey,
                ExchangeName = exchangeName
            };
            _brocker.WireProtocol.WriteMessage(new DefaultSerializer(defaultMessage.MemoryStream), payload);
            return defaultMessage;
        }
    }
}