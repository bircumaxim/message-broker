namespace Data.Models
{
    public class MessageData
    {
        public string MessageId { get; set; }
        public bool IsDurable { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeName { get; set; }
        public byte[] Payload { get; set; }
    }
}