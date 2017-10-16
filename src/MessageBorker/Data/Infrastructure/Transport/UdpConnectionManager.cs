using System.Collections.Generic;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Messages;
using Messages.Connection;
using Messages.Udp;
using Serialization;
using Serialization.WireProtocol;
using Transport.Connectors.Udp;
using Transport.Connectors.Udp.Events;

namespace Transport
{
    public class UdpConnectionManager : ConnectionManager
    {
        private readonly Dictionary<string, UdpConnector> _connectors;
        private readonly UdpReceiver _udpReceiver;
        private readonly IWireProtocol _wireProtocol;

        public UdpConnectionManager(int port, IWireProtocol wireProtocol) : base(wireProtocol)
        {
            _wireProtocol = wireProtocol;
            _connectors = new Dictionary<string, UdpConnector>();
            _udpReceiver = new UdpReceiver(port, _wireProtocol);
            _udpReceiver.UdpMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, UdpMessageReceivedEventArgs args)
        {
            UdpConnector connector;
            if (_connectors.TryGetValue(args.ConnectorName, out connector))
            {
                connector.OnNewMessageReceived(args.Message);
            } 
            else if(args.Message.MessageTypeName == typeof(UdpInitMessageRequest).Name)
            {
                var message = args.Message as UdpInitMessageRequest;
                if (message != null)
                {
                    var ipEndPoint = new IPEndPoint(IPAddress.Parse(message.ClientIp ), message.ClientPort);
                    var udpConnector = new UdpConnector(_udpReceiver.Socket, ipEndPoint, _wireProtocol);
                    _connectors.Add(args.ConnectorName, udpConnector);
                    OnNewConnection(udpConnector);
                    udpConnector.SendMessage(new UdpInitMessageResponse());
                }
            }
            if (args.Message.MessageTypeName == typeof(CloseConnectionRequest).Name)
            {
                _connectors.Remove(args.ConnectorName);
            }
        }

        public override void Start()
        {
            _udpReceiver.Start();
        }

        public override Task StartAsync()
        {
            return _udpReceiver.StartAsync();
        }

        public override void Stop()
        {
            _udpReceiver.Stop();
        }
    }
}