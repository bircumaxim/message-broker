using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using Messages.Udp;
using Serialization.Deserializer;
using Serialization.WireProtocol;
using Transport.Connectors.UdpMulticast.Events;

namespace Transport.Connectors.UdpMulticast
{
    public class UdpMulticastReceiver
    {
        private readonly IWireProtocol _wireProtocol;
        private Socket _socket;
        private readonly ILog _logger;
        private readonly ManualResetEvent _allDone;
        public event UdpMulticastMessageReceivedHandler UdpMulticastMessageReceivedHandler;

        public UdpMulticastReceiver(IPEndPoint ipEndPoint, IWireProtocol wireProtocol)
        {
            _logger = LogManager.GetLogger(GetType());
            _wireProtocol = wireProtocol;
            _allDone = new ManualResetEvent(false);
            InitUdpMulticastReceiverSocket(ipEndPoint);
        }

        public void StopReceivingMessages()
        {
            _socket.Shutdown(SocketShutdown.Send);
            _socket.Close();
            _socket.Dispose();
        }
        
        public void StartReceivingMessages()
        {
            while (_socket.IsBound)
            {
                try
                {
                    _allDone.Reset();
                    var state = new StateObject {Buffer = new byte[_socket.SendBufferSize]};
                    _socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReadCallback, state);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                _allDone.WaitOne();
            }
        }

        
        public void ReadCallback(IAsyncResult result)
        {
            if (!_socket.IsBound) return;
            _allDone.Set();
            if (_socket == null) return;
            int bytesRead = 0;
            try
            {
                bytesRead = _socket.EndReceive(result);
            }
            catch (Exception ex)
            {
                // ignored
            }
            if (bytesRead > 0)
            {
                var state = result.AsyncState as StateObject;
                if (state == null) return;
                var message =
                    _wireProtocol.ReadMessage(new DefaultDeserializer(new MemoryStream(state.Buffer, 0, bytesRead)));
                UdpMulticastMessageReceivedHandler?.Invoke(this, new UdpMulticastMessageReceivedEventArgs(message));
            }
        }
        
        private void InitUdpMulticastReceiverSocket(IPEndPoint ipEndPoint)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var localEp = new IPEndPoint(IPAddress.Any, ipEndPoint.Port);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _socket.ExclusiveAddressUse = false;

            try
            {
                _socket.Bind(localEp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(ipEndPoint.Address));
            _logger.Info($"Started Socket protocol=\"Udp multicast\" ip=\"{ipEndPoint.Address}\" port=\"{ipEndPoint.Port}\"");
        }
    }
    
    internal class StateObject
    {
        public byte[] Buffer { get; set; }
    }
}