using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Messages.Connection;
using Messages.Udp;
using Serialization;
using Serialization.WireProtocol;
using Transport.Connectors.Udp;
using Transport.Connectors.Udp.Events;

namespace MessageBuss.Brocker
{
    public class UdpBrockerClient : BrockerClient
    {
        private readonly UdpConnector _udpConnector;
        private readonly UdpReceiver _udpReceiver;

        public UdpBrockerClient(string brockerName, IWireProtocol wireProtocol, IPEndPoint endPoint,
            Dictionary<string, string> defautlExchanges) : base(brockerName, wireProtocol, defautlExchanges)
        {
            _udpReceiver = new UdpReceiver(5000, wireProtocol);
            _udpConnector = new UdpConnector(GetUdpSocket(), endPoint, wireProtocol);
            _udpReceiver.UdpMessageReceived += UdpReceiverOnUdpMessageReceived;
            _udpConnector.MessageReceived += OnMessageReceived;
        }


        #region IRun methods

        public override void Start()
        {
            _udpConnector.SendMessage(new UdpInitMessageRequest {ClientIp = "127.0.0.1", ClientPort = 5000});
            _udpReceiver.Start();
            _udpConnector.Start();
        }

        public override Task StartAsync()
        {
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

        private Socket GetUdpSocket()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            return socket;
        }
    }
}