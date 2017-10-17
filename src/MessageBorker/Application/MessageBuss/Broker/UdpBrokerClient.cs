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

        public UdpBrokerClient(string brokerName, IWireProtocol wireProtocol, IPEndPoint ipEndPoint,
            Dictionary<string, string> defautlExchanges) : base(brokerName, wireProtocol, defautlExchanges, ipEndPoint)
        {
            _udpReceiver = new UdpReceiver(5000, wireProtocol);
            _udpConnector = new UdpConnector(GetUdpSocket(), ipEndPoint, wireProtocol);
            _udpReceiver.UdpMessageReceived += UdpReceiverOnUdpMessageReceived;
            _udpConnector.MessageReceived += OnMessageReceived;
        }

        #region IRun methods

        public override void Start()
        {
            //TODO add ip and port to configuration
            _udpConnector.SendMessage(new UdpInitMessageRequest {ClientIp = "127.0.0.1", ClientPort = 5000});
            _udpReceiver.Start();
            _udpConnector.Start();
        }

        public override Task StartAsync()
        {
            //TODO add ip and port to configuration
            _udpConnector.SendMessage(new UdpInitMessageRequest {ClientIp = "127.0.0.1", ClientPort = 5000});
            _udpReceiver.StartAsync();
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
            //TODO add ip and port to configuration
            return new SubscribeMessage
            {
                Ip = "127.0.0.1",
                Port = 5000,
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