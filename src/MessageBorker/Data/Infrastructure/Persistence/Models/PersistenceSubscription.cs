namespace Persistence.Models
{
    public class PersistenceSubscription
    {
        public string SubscriberName { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
    }
}