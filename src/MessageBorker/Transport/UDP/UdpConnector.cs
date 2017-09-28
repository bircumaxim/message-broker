using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serialization;
using Serialization.Deserializers;
using Serialization.Serializers;
using Transport.Events;

namespace Transport.UDP
{
    public class UdpConnector : ConnectionLessConnector
    {
        private readonly Socket _socket;
        private readonly IWireProtocol _wireProtocol;
        private readonly ManualResetEvent _allDone;
        
        public UdpConnector(int port, long connectorId, IWireProtocol wireProtocol) : base(connectorId)
        {
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
            //TODO log here started receiving messages
            while (IsAlive)
            {
                try
                {
                    _allDone.Reset();
                    var state = new StateObject {Buffer = new byte[_socket.SendBufferSize]};
                    _socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);
                    //TODO log here that a new message was received by communicator
                }
                catch (Exception ex)
                {
                    //TODO log here exception
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
                //TODO log here exception.
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
//            Console.WriteLine(message.MessageTypeId);
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
                //TODO log here exception.
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