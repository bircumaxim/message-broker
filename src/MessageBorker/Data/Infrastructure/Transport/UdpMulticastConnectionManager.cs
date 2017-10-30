using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Messages.Connection;
using Messages.Subscribe;
using Serialization.WireProtocol;
using Transport.Connectors.UdpMulticast;
using Transport.Connectors.UdpMulticast.Events;
using Transport.Events;

namespace Transport
{
    public class UdpMulticastConnectionManager : ConnectionManager
    {
        private readonly List<string> _queuesToSubscribe;
        private readonly UdpMulticastConnector _udpMulticastConnector;

        public UdpMulticastConnectionManager(IPEndPoint ipEndPoint, IWireProtocol wireProtocol, int maxMessageLength,
            List<string> queuesToSubscribe) :
            base(wireProtocol)
        {
            _queuesToSubscribe = queuesToSubscribe;
            _udpMulticastConnector = new UdpMulticastConnector(ipEndPoint, wireProtocol, maxMessageLength);
            _udpMulticastConnector.StateChanged += OnConnectorStarted;
        }

        public override void Start()
        {
            _udpMulticastConnector.Start();
        }

        public override Task StartAsync()
        {
            return _udpMulticastConnector.StartAsync();
        }

        public override void Stop()
        {
            _udpMulticastConnector.Stop();
        }

        private void OnConnectorStarted(object sender, ConnectorStateChangeEventArgs args)
        {
            if (args.NewState == ConnectionState.Connected)
            {
                OnNewConnection(_udpMulticastConnector);
                _udpMulticastConnector.OnMessageReceived(this,
                    new UdpMulticastMessageReceivedEventArgs(new OpenConnectionRequest()));
                _queuesToSubscribe.ForEach(queue =>
                {
                    var subscribeMessage = new SubscribeMessage
                    {
                        QueueName = queue
                    };
                    _udpMulticastConnector.OnMessageReceived(this,
                        new UdpMulticastMessageReceivedEventArgs(subscribeMessage));
                });
            }
        }
    }
}