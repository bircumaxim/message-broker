using System;
using Serialization;

namespace MessageBuss.Broker.Events
{
    public delegate void BrokerClientMessageReceivedHandler(object sender, BrokerClientMessageReceivedEventArgs args);

    public class BrokerClientMessageReceivedEventArgs : EventArgs
    {
        public BrokerClient BrokerClient{ get; set; }
        public Message Message { get; set; }

        public BrokerClientMessageReceivedEventArgs(BrokerClient brokerClient, Message message)
        {
            BrokerClient = brokerClient;
            Message = message;
        }
    }
}