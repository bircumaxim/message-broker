using System;
using System.Net;
using Serialization;

namespace Transport.Connectors.Udp.Events
{
    public delegate void UdpMessageReceivedEventHandler(object sender, UdpMessageReceivedEventArgs e);

    public class UdpMessageReceivedEventArgs : EventArgs
    {
        public string ConnectorName{ get; set; }
        public IPEndPoint SenderIpEndPoint { get; set; }
        public Message Message { get; set; }

        public UdpMessageReceivedEventArgs(string connectorName, Message message)
        {
            ConnectorName = connectorName;
            Message = message;
        }
    }
}