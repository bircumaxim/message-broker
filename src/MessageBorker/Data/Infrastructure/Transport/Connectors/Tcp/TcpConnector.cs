using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;
using Serialization;
using Serialization.Deserializers;
using Serialization.Serializers;
using Transport.Events;

namespace Transport.Connectors.Tcp
{
    public class TcpConnector : ConnectionOrientedConnector
    {
        private readonly ILog _logger;
        private readonly Socket _socket;
        private readonly NetworkStream _networkStream;
        private readonly IWireProtocol _wireProtocol;

        public TcpConnector(Socket socket, long connectorId, IWireProtocol wireProtocol) : base(connectorId)
        {
            _logger = LogManager.GetLogger(GetType());
            _socket = socket;
            _wireProtocol = wireProtocol;
            _networkStream = new NetworkStream(_socket);
            Validate();
        }

        protected override void StartCommunication()
        {
            if (!_socket.Connected)
            {
                _logger.Error("Socket is not connected");
                throw new Exception("Tried to start communication with a TCP socket that is not connected.");
            }

            Task.Factory.StartNew(StartReceivingMessages);
        }
        
        protected override void StopCommunication()
        {
            if (!_socket.Connected) return;
            _socket.Shutdown(SocketShutdown.Send);
            _socket.Close();
            _socket.Dispose();
        }

        protected override void SendMessageInternal(Message message)
        {
            if (ConnectionState != ConnectionState.Connected)
            {
                _logger.Error("Socket is not connected");
                throw new Exception("Communicator's state is not connected. It can not send message.");
            }

            SendMessageToSocket(message);
        }

        private void StartReceivingMessages()
        {
            _logger.Info("Started receivin messages");
            while (ConnectionState == ConnectionState.Connected || ConnectionState == ConnectionState.Connecting)
            {
                try
                {
                    var message = _wireProtocol.ReadMessage(new DefaultDeserializer(_networkStream));
                    _logger.Info("A new message was received by communicator");
                    OnMessageReceived(message);
                }
                catch (Exception ex)
                {
                    _logger.Error("Receiving message was failed");
                    Console.WriteLine(ex);
                    break; //Stop listening
                }
            }
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to stop the connector");
                Console.WriteLine(ex);
                throw;
            }
        }
        
        private void SendMessageToSocket(Message message)
        {
            _logger.Info("Message is preparing to be sent to communicator");
            var memoryStream = new MemoryStream();
            _wireProtocol.WriteMessage(new DefaultSerializer(memoryStream), message);

            //Check the length of message data 50 MegaBytes in our case
            if (memoryStream.Length > 52428800) //TODO add settings module and put it into settings 
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
                    throw new Exception("Message can not be sent via TCP socket. Only " + totalSent + " bytes of " +
                                        length + " bytes are sent.");
                }

                totalSent += sent;
            }
            _logger.Info("Message was sent");
        }

        private void Validate()
        {
            if (_socket == null || _wireProtocol == null || _networkStream == null)
            {
                _logger.Error("Entity vaildation error");
                throw new NullReferenceException();
            }
        }
    }
}