using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using log4net;
using Messages.Udp;
using Serialization;
using Serialization.Serializer;
using Serialization.WireProtocol;

namespace Transport.Connectors.Udp
{
    public class UdpConnector : ConnectionLessConnector
    {
        private readonly ILog _logger;
        private readonly Socket _socket;
        public IPEndPoint IpEndPoint { get; set; }
        private readonly IWireProtocol _wireProtocol;

        public UdpConnector(Socket socket, IPEndPoint ipEndPoint, IWireProtocol wireProtocol)
        {
            _logger = LogManager.GetLogger(GetType());
            _socket = socket;
            IpEndPoint = ipEndPoint;
            _wireProtocol = wireProtocol;
            Validate();
        }

        protected override void StartCommunication()
        {
            IsAlive = true;
        }

        protected override void StopCommunication()
        {
            IsAlive = false;
        }

        public void OnNewMessageReceived(Message message)
        {
            OnMessageReceived(message);
        }

        protected override void SendMessageInternal(Message message)
        {
            _logger.Debug($"{message.MessageTypeName} is preparing to be sent to " +
                          $"{GetType().Name} with id=\"{ConnectorId}\"");
            MemoryStream memoryStream = new MemoryStream();
            var udpMessageWrapper = new UdpMessageWrapper {ClientName = ConnectorId};
            _wireProtocol.WriteMessage(new DefaultSerializer(udpMessageWrapper.MemoryStream), message);
            _wireProtocol.WriteMessage(new DefaultSerializer(memoryStream), udpMessageWrapper);
            _socket.SendTo(memoryStream.ToArray(), IpEndPoint);
            _logger.Info($"Sent     " +
                         $"message=\"{message.MessageTypeName}\" " +
                         $"size=\"{memoryStream.Length} byte\" to {GetType().Name} with id {ConnectorId}");
        }

        private void Validate()
        {
            if (_socket == null || _wireProtocol == null)
            {
                _logger.Error($"{GetType().Name} validation error");
                throw new NullReferenceException();
            }
        }
    }
}