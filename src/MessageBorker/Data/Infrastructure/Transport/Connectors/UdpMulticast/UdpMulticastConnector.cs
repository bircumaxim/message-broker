using System;
using System.Net;
using System.Threading.Tasks;
using log4net;
using Serialization;
using Serialization.WireProtocol;
using Transport.Connectors.UdpMulticast.Events;
using Transport.Events;

namespace Transport.Connectors.UdpMulticast
{
    public class UdpMulticastConnector : ConnectionOrientedConnector
    {
        private const int DefaultMessageLength = 52428800;
        private readonly UdpMulticastSender _udpMulticastSender;
        private readonly ILog _logger;

        public UdpMulticastConnector(IPEndPoint ipEndPoint, IWireProtocol wireProtocol,
            int maxMessageLength = DefaultMessageLength)
        {
            _logger = LogManager.GetLogger(GetType());
            _udpMulticastSender = new UdpMulticastSender(ipEndPoint, wireProtocol, maxMessageLength);
        }

        public void OnMessageReceived(object sender, UdpMulticastMessageReceivedEventArgs args)
        {
            OnMessageReceived(args.Message);
        }

        protected override void StartCommunication()
        {
//            Task.Factory.StartNew(_udpMulticastReceiver.StartReceivingMessages);
        }

        protected override void StopCommunication()
        {
//            _udpMulticastReceiver.StopReceivingMessages();
        }

        protected override void SendMessageInternal(Message message)
        {
            if (ConnectionState != ConnectionState.Connected)
            {
                _logger.Error("Socket is not connected");
                throw new Exception("Communicator's state is not connected. It can not send message.");
            }
            _logger.Debug($"{message.MessageTypeName} is preparing to be sent to " +
                          $"{GetType().Name} with id=\"{ConnectorId}\"");
            var totalSent = _udpMulticastSender.Send(message);
            _logger.Info(
                $"Sent     message=\"{message.MessageTypeName}\" size=\"{totalSent} byte\" to {GetType().Name} with id {ConnectorId}");
        }
    }
}