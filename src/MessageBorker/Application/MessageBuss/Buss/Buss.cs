using System;
using System.IO;
using MessageBuss.Broker;
using MessageBuss.Broker.Events;
using MessageBuss.Buss.Events;
using Messages.Payload;
using Messages.ServerInfo;
using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace MessageBuss.Buss
{
    public class Buss
    {
        private readonly BrokerClient _broker;
        public event MessageReceivedHandler MessageReceived;

        public Buss(BrokerClient broker)
        {
            _broker = broker;
            _broker.MessageReceivedFromBrokerHandler += OnMessageReceivedFromBroker;
        }

        #region Buss features

        public void Ping()
        {
            _broker.Ping();
        }

        public void RequestServerInfo()
        {
            _broker.SendOrEnqueue(new ServerGerneralInfoRequest());
        }

        public void Request(string queueName)
        {
            _broker.SendOrEnqueue(new PayloadRequestMessage {QueueName = queueName});
        }

        public void Publish(string exchangeName, string routingKey, Message payload, bool isDurable = false)
        {
            _broker.SendOrEnqueue(CreateRouteMessage(exchangeName, routingKey, payload, isDurable));
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

        public void Subscribe(string queueName)
        {
           _broker.Subscribe(queueName);
        }
        
        public void Unsubscribe()
        {
            _broker.Unsubscribe();
        }

        public void Dispose()
        {
            if (_broker != null)
            {
                _broker.MessageReceivedFromBrokerHandler -= OnMessageReceivedFromBroker;
                _broker.Stop();    
            }
        }

        #endregion

        private void OnMessageReceivedFromBroker(object sender, BrokerClientMessageReceivedEventArgs args)
        {
            var messageType = args.Message.MessageTypeName;
            if (messageType == typeof(PayloadMessage).Name)
            {
                var payloadMessage = args.Message as PayloadMessage;
                Message payload = null;
                if (payloadMessage != null)
                {
                    var deserializer = new DefaultDeserializer(new MemoryStream(payloadMessage.Payload));
                    payload = _broker.WireProtocol.ReadMessage(deserializer);
                }
                MessageReceived?.Invoke(this, new MessegeReceviedEventArgs(payload));
            }
            else
            {
                MessageReceived?.Invoke(this, new MessegeReceviedEventArgs(args.Message));
            }
        }

        private string GetExchangeNameForType(string exchangeType)
        {
            string exchangeName;
            _broker.DefautlExchanges.TryGetValue(exchangeType, out exchangeName);
            if (exchangeName == null)
            {
                throw new Exception($"Default exchange for {exchangeType} was not set !!!");
            }
            return exchangeName;
        }

        private Message CreateRouteMessage(string exchangeName, string routingKey, Message payload,
            bool isDurable = false)
        {
            var defaultMessage = new PayloadRouteMessage
            {
                IsDurable = isDurable,
                RoutingKey = routingKey,
                ExchangeName = exchangeName
            };
            _broker.WireProtocol.WriteMessage(new DefaultSerializer(defaultMessage.MemoryStream), payload);
            return defaultMessage;
        }
    }
}