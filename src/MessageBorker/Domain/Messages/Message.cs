using System;

namespace Domain.Messages
{
    [Serializable]
    public class Message
    {
        public string MessageId { get; set; }
        public bool IsDurable { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeName { get; set; }
        public byte[] Payload { get; set; }
    }
}