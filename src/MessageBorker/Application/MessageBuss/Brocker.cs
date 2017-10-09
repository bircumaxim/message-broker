using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Messages;
using Serialization;
using Serialization.Serializers;
using Transport.Connectors.Tcp;
using Transport.Events;

namespace MessageBuss
{
    public class Brocker
    {
        public MessageReceivedFromBrockerHandler MessageReceivedFromBrockerHandler { get; set; }
        public string Name { get; }
        public IPEndPoint IpEndPoint { get; }
        private readonly TcpConnector _tcpConnector;
        private readonly Queue<Message> _messagesToSend;
        public IWireProtocol WireProtocol { get; }

        public Dictionary<string, string> DefautlExchanges { get; }

        public Brocker(string name, IWireProtocol wireProtocol, IPEndPoint ipEndPoint,
            Dictionary<string, string> defautlExchanges)
        {
            Name = name;
            IpEndPoint = ipEndPoint;
            DefautlExchanges = defautlExchanges;
            WireProtocol = wireProtocol;
            _tcpConnector = new TcpConnector(GetTcpSocket(), WireProtocol);
            _tcpConnector.StateChanged += OnStateChange;
            _tcpConnector.MessageReceived += OnMessageReceived;
            _messagesToSend = new Queue<Message>();
            _messagesToSend.Enqueue(new OpenConnectionMessage {ClientName = "Huinea"});
        }

        public void Start()
        {
            _tcpConnector.StartAsync();
        }
        
        public void Stop()
        {
            _tcpConnector.SendMessage(new CloseConnectionMessage());
            _tcpConnector.StateChanged -= OnStateChange;
            _tcpConnector.MessageReceived -= OnMessageReceived;
            _tcpConnector.Stop();
        }

        public void Send(Message message)
        {
            if (_tcpConnector.ConnectionState == ConnectionState.Connected)
            {
                _tcpConnector.SendMessage(message);
            }
            else
            {
                _messagesToSend.Enqueue(message);
            }
        }

        public void Ping()
        {
            Send(new PingMessage());
        }

        private void OnStateChange(object sender, ConnectorStateChangeEventArgs args)
        {
            if (args.NewState == ConnectionState.Connected)
            {
                while (_messagesToSend.Count > 0)
                {
                    _tcpConnector.SendMessage(_messagesToSend.Dequeue());
                }
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            switch (args.Message.MessageTypeName)
            {
                case "PingMessage":
                    Send(new PongMessage());
                    break;
                case "PongMessage":
                    //TODO add logs here.
                    break;
            }
            
            var messageReceivedFromBrockerEventArgs = new MessageReceivedFromBrockerEventArgs(this, args.Message);
            MessageReceivedFromBrockerHandler?.Invoke(this, messageReceivedFromBrockerEventArgs);
        }

        private Socket GetTcpSocket()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(IpEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return socket;
        }
    }
}