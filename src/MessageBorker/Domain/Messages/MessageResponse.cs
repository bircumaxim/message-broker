namespace Domain.Messages
{
    public class MessageResponse
    {
        public string MessageId { get; set; }
        public string ReceiverName { get; set; }
        public byte[] Payload { get; set; }
    }
}