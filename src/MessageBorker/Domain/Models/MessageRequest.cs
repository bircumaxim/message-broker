namespace Domain.Models
{
    public class MessageRequest
    {
        public string QueueName { get; set; }
        public string ReceiverName { get; set; }
    }
}