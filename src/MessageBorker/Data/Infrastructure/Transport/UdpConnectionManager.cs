using System.Threading.Tasks;
using log4net;
using Serialization;
using Transport.Connectors.Udp;
using Transport.Events;

namespace Transport
{
    public class UdpConnectionManager : ConnectionManager
    {
        private readonly UdpReceiver _udpReceiver;
        
        public UdpConnectionManager(int port, IWireProtocol wireProtocol) : base(wireProtocol)
        {
            _udpReceiver = new UdpReceiver(port, wireProtocol);
            _udpReceiver.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            if (args.Message.MessageTypeName == "OpenConnectionMessage")
            {
                
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