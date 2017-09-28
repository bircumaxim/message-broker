using System;
using Transport.Serialization;

namespace Transport.Events
{
    public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs : EventArgs
    {
        public IConnector Communicator { get; set; }
        public Message Message { get; set; }

        public MessageReceivedEventArgs(IConnector communicator, Message message)
        {
            Communicator = communicator;
            Message = message;
        }
    }
}