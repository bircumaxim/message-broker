using System;
using Messages;
using Serialization;
using Serialization.Serializers;

namespace MessageBuss
{
    public class Buss
    {
        private readonly Brocker _brocker;

        public Buss(Brocker brocker)
        {
            _brocker = brocker;
        }

        public void Publish(string exchangeName, string routingKey, Message payload, bool isDurable = false)
        {
            _brocker.Send(CreateDefaultMessage(exchangeName, routingKey, payload, isDurable));
        }

        public void Ping()
        {
            _brocker.Ping();
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

        private Message CreateDefaultMessage(string exchangeName, string routingKey, Message payload, bool isDurable = false)
        {
            var defaultMessage = new DefaultMessage
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