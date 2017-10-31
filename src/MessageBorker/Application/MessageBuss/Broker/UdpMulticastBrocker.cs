using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Messages.Subscribe;
using Serialization;
using Serialization.WireProtocol;
using Transport.Connectors.UdpMulticast;
using Transport.Connectors.UdpMulticast.Events;
using Transport.Events;

namespace MessageBuss.Broker
{
    public class UdpMulticastBrocker : BrokerClient
    {
        private readonly UdpMulticastConnector _udpMulticastConnector;
        private readonly UdpMulticastReceiver _udpMulticastReceiver;

        public UdpMulticastBrocker(string brokerName, IWireProtocol wireProtocol, IPEndPoint connectorIpEndpoint,
            Dictionary<string, string> defautlExchanges) : base(brokerName, wireProtocol,
            defautlExchanges, connectorIpEndpoint)
        {
            _udpMulticastConnector = new UdpMulticastConnector(connectorIpEndpoint, wireProtocol);
            _udpMulticastReceiver = new UdpMulticastReceiver(connectorIpEndpoint, wireProtocol);
            _udpMulticastReceiver.UdpMulticastMessageReceivedHandler += OnMessageReceived;
        }

        public override void Start()
        {
            _udpMulticastConnector.Start();
        }

        public override Task StartAsync()
        {
            Task.Factory.StartNew(_udpMulticastReceiver.StartReceivingMessages);
            return _udpMulticastConnector.StartAsync();
        }

        public override void Stop()
        {
            _udpMulticastConnector.Stop();
        }

        protected override void SendMessageToConnector(Message message)
        {
            //TODO fix udpMulticast sneder issue.
//            _udpMulticastConnector.SendMessage(message);
        }

        private void OnMessageReceived(object sender, UdpMulticastMessageReceivedEventArgs args)
        {
            OnMessageReceived(this, new MessageReceivedEventArgs(_udpMulticastConnector, args.Message));
        }
        
        protected override Message GetSubscribtionMessage(string queueName)
        {
            //TODO add ip and port to configuration
            return new SubscribeMessage
            {
                Ip = "127.0.0.1",
                Port = 5000,
                QueueName = queueName,
                IsDurable = true
            };
        }
    }
}