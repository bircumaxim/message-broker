using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Serialization;
using Serialization.Deserializers;
using Serialization.Serializers;

namespace Transport.Connectors.Udp
{
    public class UdpConnector : ConnectionLessConnector
    {
        private readonly ILog _logger;
        private readonly Socket _socket;
        private readonly IWireProtocol _wireProtocol;
        private readonly ManualResetEvent _allDone;

        public UdpConnector(int port, IWireProtocol wireProtocol)
        {
            _logger = LogManager.GetLogger(GetType());
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _wireProtocol = wireProtocol;
            _allDone = new ManualResetEvent(false);
            _socket.Bind(new IPEndPoint(0, port));
            Validate();
        }

        protected override void StartCommunication()
        {
            Task.Factory.StartNew(StartReceivingMessages);
        }

        protected override void StopCommunication()
        {
            if (!IsAlive) return;
            _socket.Close();
            _socket.Dispose();
        }

        private void StartReceivingMessages()
        {
            _logger.Info($"Conector with {ConnectorId} Started receiving messages");
            while (IsAlive)
            {
                try
                {
                    _allDone.Reset();
                    var state = new StateObject {Buffer = new byte[_socket.SendBufferSize]};
                    _socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Connector {ConnectorId} failed to receive message !");
                    Console.WriteLine(ex);
                    break; //Stop listening
                }
                _allDone.WaitOne();
            }
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                _logger.Error($"Connector {ConnectorId} failed to stop !");
                Console.WriteLine(ex);
                throw;
            }
        }

        public void ReadCallback(IAsyncResult result)
        {
            _allDone.Set();
            var bytesRead = _socket.EndReceive(result);
            var state = result.AsyncState as StateObject;
            if (state == null) return;
            var memoryStream = new MemoryStream(state.Buffer, 0, bytesRead);
            var message = _wireProtocol.ReadMessage(new DefaultDeserializer(memoryStream));
            if (message != null)
            {
                OnMessageReceived(message);
            }
        }

        protected override void SendMessageInternal(Message message, EndPoint endPoint)
        {
            var memoryStream = new MemoryStream();
            _wireProtocol.WriteMessage(new DefaultSerializer(memoryStream), message);
            _socket.SendTo(memoryStream.ToArray(), endPoint);
        }

        private void Validate()
        {
            if (_socket == null || _allDone == null || _wireProtocol == null)
            {
                _logger.Error($"Entity validation failed");
                throw new NullReferenceException();
            }
        }
    }

    internal class StateObject
    {
        public byte[] Buffer { get; set; }

        // Received data string.
        public readonly StringBuilder StringBuilder = new StringBuilder();
    }
}