using System;
using Serialization;

namespace Transport.Events
{
    public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs : EventArgs
    {
        public IConnector Connector { get; set; }
        public Message Message { get; set; }

        public MessageReceivedEventArgs(IConnector connector, Message message)
        {
            Connector = connector;
            Message = message;
        }
    }
}