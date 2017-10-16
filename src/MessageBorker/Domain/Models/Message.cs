namespace Domain.Models
{
    public class Message
    {
        public string MessageId { get; set; }
        public string ReceiverName { get; set; }
        public byte[] Payload { get; set; }
    }
}