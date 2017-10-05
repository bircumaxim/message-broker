using Data.Models;

namespace Data.Events
{
    public delegate void MessageReceivedFromRemoteApplicationHandler(object sender, MessageReceivedFromRemoteApplicationEventArgs e);
    
    public class MessageReceivedFromRemoteApplicationEventArgs
    {
        public RemoteApplication Application { get; set; }
        public MessageData MessageData { get; set; }
    }
}