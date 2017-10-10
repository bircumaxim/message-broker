using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessageBuss.Brocker.Events;
using Messages;
using Messages.Connection;
using Messages.ServerInfo;
using Serialization;
using Transport.Connectors.Tcp;
using Transport.Events;

namespace MessageBuss.Brocker
{
    public class BrockerClient : IRun
    {
        public string BrockerName { get; }
        private bool _isConnectionAccepted;
        public IWireProtocol WireProtocol { get; }
        private readonly TcpConnector _tcpConnector;
        private readonly Queue<Message> _messagesToSend;
        public Dictionary<string, string> DefautlExchanges { get; }
        public event BrockerClientMessageReceivedHandler MessageReceivedFromBrockerHandler;

        public BrockerClient(string brockerName, IWireProtocol wireProtocol, IPEndPoint ipEndPoint,
            Dictionary<string, string> defautlExchanges)
        {
            BrockerName = brockerName;
            DefautlExchanges = defautlExchanges;
            WireProtocol = wireProtocol;
            _messagesToSend = new Queue<Message>();
            _tcpConnector = new TcpConnector(GetTcpSocket(ipEndPoint), WireProtocol);
            _tcpConnector.StateChanged += OnStateChange;
            _tcpConnector.MessageReceived += OnMessageReceived;
        }

        #region Irun methods

        public void Start()
        {
            _tcpConnector.Start();
        }

        public Task StartAsync()
        {
            return _tcpConnector.StartAsync();
        }

        public void Stop()
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

        public void SendOrEnqueue(Message message)
        {
            if (_tcpConnector.ConnectionState != ConnectionState.Connected || !_isConnectionAccepted)
            {
                _messagesToSend.Enqueue(message);
            }

            else
            {
                _tcpConnector.SendMessage(message);
            }
        }

        public void Ping()
        {
            SendOrEnqueue(new PingMessage());
        }

        #region Listeners

        private void OnStateChange(object sender, ConnectorStateChangeEventArgs args)
        {
            if (args.NewState == ConnectionState.Connected)
            {
                _tcpConnector.SendMessage(new OpenConnectionRequest());
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            switch (args.Message.MessageTypeName)
            {
                case "OpenConnectionResponse":
                    OnOpenConecctionResponse(args.Message as OpenConnectionResponse);
                    break;
                case "CloseConnectionResponse":
                    OnCloseConecctionResponse();
                    break;
                case "PingMessage":
                    SendOrEnqueue(new PongMessage());
                    break;
                default:
                    var message = new BrockerClientMessageReceivedEventArgs(this, args.Message);
                    MessageReceivedFromBrockerHandler?.Invoke(this, message);
                    break;
            }
        }

        #endregion

        private void OnCloseConecctionResponse()
        {
           Stop();
        }

        private void OnOpenConecctionResponse(OpenConnectionResponse response)
        {
            _isConnectionAccepted = response.IsConnectionAccepted;
            while (_messagesToSend.Count > 0)
            {
                _tcpConnector.SendMessage(_messagesToSend.Dequeue());
            }
        }

        private Socket GetTcpSocket(IPEndPoint endPoint)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(endPoint);
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