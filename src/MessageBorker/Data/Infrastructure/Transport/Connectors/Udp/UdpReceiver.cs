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
using Transport.Events;

namespace Transport.Connectors.Udp
{
    public class UdpReceiver : IRun
    {
        private readonly ILog _logger;
        private readonly Socket _socket;
        private readonly IWireProtocol _wireProtocol;
        private readonly ManualResetEvent _allDone;
        public event MessageReceivedHandler MessageReceived;
        private bool _isAlive;

        public UdpReceiver(int port, IWireProtocol wireProtocol)
        {
            _logger = LogManager.GetLogger(GetType());
            _isAlive = false;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _wireProtocol = wireProtocol;
            _allDone = new ManualResetEvent(false);
            _socket.Bind(new IPEndPoint(0, port));
            Validate();
        }

        public void Start()
        {
            if (_isAlive)
            {
                throw new Exception("Receiver is already started");
            }
            StartReceivingMessages();
        }

        public Task StartAsync()
        {
            if (_isAlive)
            {
                throw new Exception("Receiver is already started");
            }
            _isAlive = true;
            return Task.Factory.StartNew(StartReceivingMessages);
        }

        public void Stop()
        {
            if (!_isAlive) return;
            _isAlive = false;
            _socket.Close();
            _socket.Dispose();
        }

        private void StartReceivingMessages()
        {
            while (_isAlive)
            {
                try
                {
                    _allDone.Reset();
                    var state = new StateObject {Buffer = new byte[_socket.SendBufferSize]};
                    _socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);
                }
                catch (Exception ex)
                {
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

        private void Validate()
        {
            if (_socket == null || _allDone == null || _wireProtocol == null)
            {
                _logger.Error($"Entity validation failed");
                throw new NullReferenceException();
            }
        }


        protected void OnMessageReceived(Message message)
        {
//            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(this, message));
        }
    }

    internal class StateObject
    {
        public byte[] Buffer { get; set; }

        // Received data string.
        public readonly StringBuilder StringBuilder = new StringBuilder();
    }
}