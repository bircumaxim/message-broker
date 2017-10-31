using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Messages.Connection;
using Messages.Subscribe;
using Messages.Udp;
using Serialization;
using Serialization.WireProtocol;
using Transport.Connectors.Udp;
using Transport.Connectors.Udp.Events;

namespace MessageBuss.Broker
{
    public class UdpBrokerClient : BrokerClient
    {
        private readonly UdpConnector _udpConnector;
        private readonly UdpReceiver _udpReceiver;
        private readonly IPEndPoint _receiverIpEndPoint;

        public UdpBrokerClient(string brokerName,
            IWireProtocol wireProtocol,
            IPEndPoint connectorIpEndpoint,
            IPEndPoint receiverIpEndPoint,
            Dictionary<string, string> defautlExchanges) : base(brokerName, wireProtocol, defautlExchanges,
            connectorIpEndpoint)
        {
            _receiverIpEndPoint = receiverIpEndPoint;
            _udpReceiver = new UdpReceiver(_receiverIpEndPoint.Port, wireProtocol);
            _udpConnector = new UdpConnector(GetUdpSocket(), connectorIpEndpoint, wireProtocol);
            _udpReceiver.UdpMessageReceived += UdpReceiverOnUdpMessageReceived;
            _udpConnector.MessageReceived += OnMessageReceived;
        }

        #region IRun methods

        public override void Start()
        {
            _udpConnector.SendMessage(new UdpInitMessageRequest
            {
                ClientIp = _receiverIpEndPoint.Address.ToString(),
                ClientPort = _receiverIpEndPoint.Port
            });
            _udpReceiver.Start();
            _udpConnector.Start();
        }

        public override Task StartAsync()
        {
            _udpReceiver.StartAsync();
            _udpConnector.SendMessage(new UdpInitMessageRequest
            {
                ClientIp = _receiverIpEndPoint.Address.ToString(),
                ClientPort = _receiverIpEndPoint.Port
            });
            return _udpConnector.StartAsync();
        }

        public override void Stop()
        {
            _udpConnector.SendMessage(new CloseConnectionRequest());
            _udpConnector.MessageReceived -= OnMessageReceived;
            _udpReceiver.Stop();
            _udpConnector.Stop();
        }

        #endregion

        private void UdpReceiverOnUdpMessageReceived(object sender, UdpMessageReceivedEventArgs args)
        {
            _udpConnector.OnNewMessageReceived(args.Message);
        }

        protected override void SendMessageToConnector(Message message)
        {
            _udpConnector.SendMessage(message);
        }

        protected override Message GetSubscribtionMessage(string queueName)
        {
            return new SubscribeMessage
            {
                Ip = _receiverIpEndPoint.Address.ToString(),
                Port = _receiverIpEndPoint.Port,
                QueueName = queueName,
                IsDurable = true
            };
        }

        private Socket GetUdpSocket()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            return socket;
        }
    }
}