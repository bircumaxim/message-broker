using System;

namespace Domain.Messages
{
    [Serializable]
    public class Message
    {
        public bool IsDurable { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeName { get; set; }
        public byte[] Payload { get; set; }
    }
}