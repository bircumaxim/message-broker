using System.IO;
using System.Net;
using System.Net.Sockets;
using Serialization;
using Serialization.Serializers;

namespace Transport.Connectors.Udp
{
    public class UdpConnector : ConnectionLessConnector
    {
        private readonly IPEndPoint _ipEndPoint;
        private readonly Socket _socket;
        private readonly IWireProtocol _wireProtocol;

        public UdpConnector(Socket socket, IPEndPoint ipEndPoint, IWireProtocol wireProtocol)
        {
            _socket = socket;
            _ipEndPoint = ipEndPoint;
            _wireProtocol = wireProtocol;
        }

        protected override void StartCommunication()
        {
            IsAlive = true;
        }

        protected override void StopCommunication()
        {
            IsAlive = false;
        }

        protected override void SendMessageInternal(Message message)
        {
            var memoryStream = new MemoryStream();
            _wireProtocol.WriteMessage(new DefaultSerializer(memoryStream), message);
            _socket.SendTo(memoryStream.ToArray(), _ipEndPoint);
        }
    }
}