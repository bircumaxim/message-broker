using System;

namespace Persistence.Models
{
    public class PersistenceMessage
    {
        public string DestinationQueueName { get; set; }
        public DateTime Timestamp { get; set; }
        public string MessageId { get; set; }
        public byte[] Payload { get; set; }
    }
}