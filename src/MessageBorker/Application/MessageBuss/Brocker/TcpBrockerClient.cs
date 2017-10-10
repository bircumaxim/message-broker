using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Messages.Connection;
using Serialization;
using Transport.Connectors.Tcp;
using Transport.Events;

namespace MessageBuss.Brocker
{
    public class TcpBrockerClient : BrockerClient
    {
        private readonly TcpConnector _tcpConnector;

        public TcpBrockerClient(string brockerName, IWireProtocol wireProtocol, IPEndPoint ipEndPoint,
            Dictionary<string, string> defautlExchanges) : base(brockerName, wireProtocol, defautlExchanges)
        {
            _tcpConnector = new TcpConnector(GetTcpSocket(ipEndPoint), WireProtocol);
            _tcpConnector.StateChanged += OnStateChange;
            _tcpConnector.MessageReceived += OnMessageReceived;
        }

        #region IRun methods

        public override void Start()
        {
            _tcpConnector.Start();
        }

        public override Task StartAsync()
        {
            return _tcpConnector.StartAsync();
        }

        public override void Stop()
        {
            if (_tcpConnector.ConnectionState == ConnectionState.Connected)
            {
                _tcpConnector.SendMessage(new CloseConnectionRequest());
                _tcpConnector.StateChanged -= OnStateChange;
                _tcpConnector.MessageReceived -= OnMessageReceived;
                _tcpConnector.Stop();
            }
        }

        #endregion

        protected override void SendMessageToConnector(Message message)
        {
            _tcpConnector.SendMessage(message);
        }
        
        private void OnStateChange(object sender, ConnectorStateChangeEventArgs args)
        {
            if (args.NewState == ConnectionState.Connected)
            {
                _tcpConnector.SendMessage(new OpenConnectionRequest());
            }
        }
        
        private Socket GetTcpSocket(IPEndPoint endPoint)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(endPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nConnection to server failed.\n" +
                                  $"Check please that on {endPoint} " +
                                  "there is a running instance of message brocker server.");
                throw ex;
            }
            return socket;
        }
    }
}