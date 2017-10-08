using Serialization;

namespace Data.Events
{
    public delegate void MessageReceivedFromRemoteApplicationHandler(object sender, MessageReceivedFromRemoteApplicationEventArgs e);
    
    public class MessageReceivedFromRemoteApplicationEventArgs
    {
        public RemoteApplication Application { get; set; }
        public Message Message { get; set; }
    }
}