using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageBuss.Brocker.Events;
using Messages.Connection;
using Messages.ServerInfo;
using Serialization;
using Transport.Events;

namespace MessageBuss.Brocker
{
    public abstract class BrockerClient : IRun
    {
        public string BrockerName { get; }
        private bool _isConnectionAccepted;
        public IWireProtocol WireProtocol { get; }
        private readonly Queue<Message> _messagesToSend;
        public Dictionary<string, string> DefautlExchanges { get; }
        public event BrockerClientMessageReceivedHandler MessageReceivedFromBrockerHandler;

        protected BrockerClient(string brockerName, IWireProtocol wireProtocol,
            Dictionary<string, string> defautlExchanges)
        {
            BrockerName = brockerName;
            DefautlExchanges = defautlExchanges;
            WireProtocol = wireProtocol;
            _messagesToSend = new Queue<Message>();
        }

        #region IRun methods

        public abstract void Start();

        public abstract Task StartAsync();

        public abstract void Stop();

        #endregion

        public void SendOrEnqueue(Message message)
        {
            if (!_isConnectionAccepted)
            {
                _messagesToSend.Enqueue(message);
            }
        }

        public void Ping()
        {
            SendOrEnqueue(new PingMessage());
        }

        protected abstract void SendMessageToConnector(Message message);

        #region Listeners

        protected void OnMessageReceived(object sender, MessageReceivedEventArgs args)
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
                case "UdpInitMessageResponse":
                    SendMessageToConnector(new OpenConnectionRequest());
                    break;
                default:
                    var message = new BrockerClientMessageReceivedEventArgs(this, args.Message);
                    MessageReceivedFromBrockerHandler?.Invoke(this, message);
                    break;
            }
        }

        private void OnCloseConecctionResponse()
        {
            Stop();
        }

        private void OnOpenConecctionResponse(OpenConnectionResponse response)
        {
            _isConnectionAccepted = response.IsConnectionAccepted;
            while (_messagesToSend.Count > 0)
            {
                SendMessageToConnector(_messagesToSend.Dequeue());
            }
        }

        #endregion
    }
}