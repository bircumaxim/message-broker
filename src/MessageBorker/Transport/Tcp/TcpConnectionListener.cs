using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Transport.Tcp.Events;

namespace Transport.Tcp
{
    public class TcpConnectionListener : IRun
    {
        private readonly int _port;
        private readonly Socket _listenerSocket;
        private readonly ManualResetEvent _allDone;
        public bool IsListening { get; set; }
        public event TcpClientConnectedHandler TcpClientConnected;

        public TcpConnectionListener(int port)
        {
            _port = port;
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _allDone = new ManualResetEvent(false);
            Validate();
        }


        public void Start()
        {
            //TODO log here that socket is starting
            BindSocketToEndpoint(new IPEndPoint(0, _port));
            IsListening = !IsListening;
            TcpSocketListening();
        }

        public Task StartAsync()
        {
            //TODO log here that socket is starting
            BindSocketToEndpoint(new IPEndPoint(0, _port));
            IsListening = !IsListening;
            return Task.Factory.StartNew(TcpSocketListening);
        }

        public void Stop()
        {
            //TODO log here that socket was stoped
            IsListening = !IsListening;
            _listenerSocket.Close();
            _listenerSocket.Dispose();
            Console.WriteLine("Socket was closed");
        }

        private void BindSocketToEndpoint(EndPoint endPoint)
        {
            try
            {
                _listenerSocket.Bind(endPoint);
                _listenerSocket.Listen(100);
            }
            catch (Exception e)
            {
                //TODO log herer socket binding exception
                Console.WriteLine(e);
                throw;
            }
        }

        private void TcpSocketListening()
        {
            while (IsListening)
            {
                _allDone.Reset();
                //TODO log here waiting for a connection
                _listenerSocket.BeginAccept(OnSocketAccepted, _listenerSocket);
                _allDone.WaitOne();
            }
        }

        private void OnSocketAccepted(IAsyncResult result)
        {
            _allDone.Set();
            var socketListener = (Socket) result.AsyncState;
            var accptedSocket = socketListener.EndAccept(result);
            //TODO log here that new client socket was accepted
            TcpClientConnected?.Invoke(this, new TcpClientConnectedEventArgs {ClientSocket = accptedSocket});
        }

        private void Validate()
        {
            if (!IsPortAvailable(_port))
            {
                //TODO log here exception
                throw new Exception("Given port is not available");
            }
        }
        
        private bool IsPortAvailable(int port)
        {
            return IPGlobalProperties
                .GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .All(tcpConnectionInformation => tcpConnectionInformation.LocalEndPoint.Port != port);
        }
    }
}