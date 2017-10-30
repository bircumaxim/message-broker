using Serialization;

namespace Transport.Connectors.UdpMulticast.Events
{
    public delegate void UdpMulticastMessageReceivedHandler(object sender, UdpMulticastMessageReceivedEventArgs args);
    
    public class UdpMulticastMessageReceivedEventArgs
    {
        public Message Message { get; set; }

        public UdpMulticastMessageReceivedEventArgs(Message message)
        {
            Message = message;
        }
    }
}