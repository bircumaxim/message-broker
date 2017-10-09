using Serialization;

namespace MessageBuss.Buss.Events
{
    public delegate void MessageReceivedHandler(object sender, MessegeReceviedEventArgs args);
    
    public class MessegeReceviedEventArgs
    {
        public Message Payload { get; set; }

        public MessegeReceviedEventArgs(Message payload)
        {
            Payload = payload;
        }
    }
}