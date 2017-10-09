using System;
using Serialization;

namespace MessageBuss.Brocker.Events
{
    public delegate void BrockerClientMessageReceivedHandler(object sender, BrockerClientMessageReceivedEventArgs args);

    public class BrockerClientMessageReceivedEventArgs : EventArgs
    {
        public BrockerClient BrockerClient{ get; set; }
        public Message Message { get; set; }

        public BrockerClientMessageReceivedEventArgs(BrockerClient brockerClient, Message message)
        {
            BrockerClient = brockerClient;
            Message = message;
        }
    }
}