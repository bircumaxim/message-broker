using Serialization;

namespace Data.Events
{
    public delegate void RemoteApplicationMessageReceived(object sender, RemoteApplicationMessageReceivedEventArgs e);
    
    public class RemoteApplicationMessageReceivedEventArgs
    {
        public RemoteApplication Application { get; set; }
        public Message Message { get; set; }
    }
}