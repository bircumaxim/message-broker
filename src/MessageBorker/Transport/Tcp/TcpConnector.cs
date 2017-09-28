using System;
using System.IO;
using System.Net.Sockets;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Serialization;
using Serialization.Deserializers;
using Serialization.Serializers;
using Transport.Events;

namespace Transport.Tcp
{
    public class TcpConnector : Connector
    {
        private readonly Socket _socket;
        private readonly NetworkStream _networkStream;
        private readonly IWireProtocol _wireProtocol;
        private readonly ManualResetEvent _allDone;

        public TcpConnector(Socket socket, long connectorId, IWireProtocol wireProtocol) : base(connectorId)
        {
            _socket = socket;
            _wireProtocol = wireProtocol;
            _allDone = new ManualResetEvent(false);
            _networkStream = new NetworkStream(_socket);
        }

        protected override void StartCommunicaiton()
        {
            if (!_socket.Connected)
            {
                //TODO log here exception socket is not connected
                throw new Exception("Tried to start communication with a TCP socket that is not connected.");
            }

            Task.Factory.StartNew(StartReceivingMessages);
        }
        
        private void StartReceivingMessages()
        {
            //TODO log here started receiving messages
            while (ConnectionState == ConnectionState.Connected || ConnectionState == ConnectionState.Connecting)
            {
                try
                {
                    var message = _wireProtocol.ReadMessage(new DefaultDeserializer(_networkStream));
                    //TODO log here that a new message was received by communicator
                    OnMessageReceived(message);
                }
                catch (Exception ex)
                {
                    //TODO log here exception
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
                //TODO log here exception.
                Console.WriteLine(ex);
                throw;
            }
        }

        protected override void StopCommunicaiton()
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
                //TODO log here exception that socket is not connected
                throw new Exception("Communicator's state is not connected. It can not send message.");
            }

            SendMessageToSocket(message);
        }

        private void SendMessageToSocket(Message message)
        {
            //TODO log here that Message is preparing to be sent to communicator

            //Create MemoryStream to write message to a byte array
            var memoryStream = new MemoryStream();

            //Write message
            _wireProtocol.WriteMessage(new DefaultSerializer(memoryStream), message);

            //Check the length of message data 50 MegaBytes in our case
            if (memoryStream.Length > 52428800) //TODO add settings module and put it into settings 
            {
                //TODO log here exception
                throw new Exception("Message is too big to send.");
            }

            //SendMessage message (contents of created memory stream)
            var sendBuffer = memoryStream.ToArray();
            var length = sendBuffer.Length;
            var totalSent = 0;
            while (totalSent < length)
            {
                var sent = _socket.Send(sendBuffer, totalSent, length - totalSent, SocketFlags.None);
                if (sent <= 0)
                {
                    //TODO log here exception
                    throw new Exception("Message can not be sent via TCP socket. Only " + totalSent + " bytes of " +
                                        length + " bytes are sent.");
                }

                totalSent += sent;
            }

            //TODO log here that message was sent.
        }
    }

    public class StateObject
    {
        // Size of receive buffer.
        public const int BufferSize = 1024;

        // Receive buffer.
        public byte[] Buffer = new byte[BufferSize];
    }
}