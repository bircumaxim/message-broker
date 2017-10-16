using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using MessageBuss.Brocker.Events;
using Messages;
using Messages.Connection;
using Messages.Payload;
using Messages.ServerInfo;
using Microsoft.SqlServer.Server;
using Serialization;
using Serialization.WireProtocol;
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
        public IPEndPoint IpEndPoint { get; }

        protected BrockerClient(string brockerName, IWireProtocol wireProtocol,
            Dictionary<string, string> defautlExchanges, IPEndPoint ipEndPoint)
        {
            BrockerName = brockerName;
            DefautlExchanges = defautlExchanges;
            IpEndPoint = ipEndPoint;
            WireProtocol = wireProtocol;
            _messagesToSend = new Queue<Message>();
        }

        #region IRun methods

        public abstract void Start();

        public abstract Task StartAsync();

        public abstract void Stop();

        #endregion

        public void Ping()
        {
            SendOrEnqueue(new PingMessage());
        }

        public void SendOrEnqueue(Message message)
        {
            if (!_isConnectionAccepted)
            {
                _messagesToSend.Enqueue(message);
            }
            else
            {
                SendMessageToConnector(message);
            }
        }

        public void Subscribe(string queueName)
        {
            SendOrEnqueue(GetSubscribtionMessage(queueName));
        }
        
        protected abstract void SendMessageToConnector(Message message);

        protected abstract Message GetSubscribtionMessage(string queueName);

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
                    if (args.Message.MessageTypeName == typeof(PayloadMessage).Name)
                    {
                        SendMessageReceivedAcknoledge(args.Message);
                    }
                    var message = new BrockerClientMessageReceivedEventArgs(this, args.Message);
                    MessageReceivedFromBrockerHandler?.Invoke(this, message);
                    break;
            }
        }

        private void SendMessageReceivedAcknoledge(Message message)
        {
            var payloadMessage = message as PayloadMessage;
            SendOrEnqueue(new PayloadMessageReceived {MessageId = payloadMessage?.MessageId});
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