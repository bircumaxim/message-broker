using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using log4net;
using Serialization;
using Serialization.Serializer;
using Serialization.WireProtocol;

namespace Transport.Connectors.UdpMulticast
{
    public class UdpMulticastSender
    {
        private readonly IWireProtocol _wireProtocol;
        private Socket _socket;
        private readonly ILog _logger;
        private readonly long _maxMessageLength;

        public UdpMulticastSender(IPEndPoint endPoint, IWireProtocol wireProtocol, long maxMessageLength)
        {
            _wireProtocol = wireProtocol;
            _maxMessageLength = maxMessageLength;
            _logger = LogManager.GetLogger(GetType());
            InitSocket(endPoint);
        }

        public int Send(Message message)
        {
            if (!_socket.Connected)
            {
                _logger.Error("Socket is not connected to a multicast group");
                throw new Exception("Connector's state is not connected. It can not send message.");
            }   
            return SendMessageToSocket(message);
        }

        private void InitSocket(IPEndPoint ipEndPoint)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(ipEndPoint.Address));
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            try
            {
                _socket.Connect(ipEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private int SendMessageToSocket(Message message)
        {
            var memoryStream = new MemoryStream();
            _wireProtocol.WriteMessage(new DefaultSerializer(memoryStream), message);

            if (memoryStream.Length > _maxMessageLength)
            {
                _logger.Error("Message is too big to send.");
                throw new Exception("Message is too big to send.");
            }

            var sendBuffer = memoryStream.ToArray();
            var length = sendBuffer.Length;
            var totalSent = 0;
            while (totalSent < length)
            {
                var sent = _socket.Send(sendBuffer, totalSent, length - totalSent, SocketFlags.None);
                if (sent <= 0)
                {
                    _logger.Error("Message can not be sent via TCP socket. Only " + totalSent + " bytes of " +
                                  length + " bytes are sent.");
                }

                totalSent += sent;
            }
            return totalSent;
        }
    }
}