namespace Data.Events
{
    public delegate void QueueUpdatedEventHandler(object sender, QueueUpdateEventArgs args);
    
    public class QueueUpdateEventArgs
    {
        public string QueueName { get; set; }

        public QueueUpdateEventArgs(string queueName)
        {
            QueueName = queueName;
        }
    }
}