using System;
using Serialization;

namespace MessageBuss
{
    public delegate void MessageReceivedFromBrockerHandler(object sender, MessageReceivedFromBrockerEventArgs e);

    public class MessageReceivedFromBrockerEventArgs : EventArgs
    {
        public Brocker Brocker{ get; set; }
        public Message Message { get; set; }

        public MessageReceivedFromBrockerEventArgs(Brocker brocker, Message message)
        {
            Brocker = brocker;
            Message = message;
        }
    }
}