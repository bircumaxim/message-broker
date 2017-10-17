using System;

namespace Domain.Models
{
    public class RouteMessage : IMessage
    {
        public string MessageId { get; set; }
        public bool IsDurable { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeName { get; set; }
        public byte[] Payload { get; set; }

        public RouteMessage()
        {
            MessageId = Guid.NewGuid().ToString();
        }

        public RouteMessage Duplicate()
        {
            return new RouteMessage
            {
                IsDurable = IsDurable,
                RoutingKey = RoutingKey,
                ExchangeName = ExchangeName,
                Payload = Payload
            };
        }

        public IMessage MakeCopy()
        {
            return new RouteMessage
            {
                IsDurable = IsDurable,
                RoutingKey = RoutingKey,
                ExchangeName = ExchangeName,
                Payload = Payload
            };
        }
    }
}