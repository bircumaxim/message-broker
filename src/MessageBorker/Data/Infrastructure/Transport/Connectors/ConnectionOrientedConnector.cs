﻿using System;
using log4net;
using Serialization;
using Transport.Events;

namespace Transport.Connectors
{
    public abstract class ConnectionOrientedConnector : Connector, IConnectionOrientedConnector
    {
        private readonly ILog _logger;
        private readonly object _sendLock;
        public event ConnectorStateChangeHandler StateChanged;
        public ConnectionState ConnectionState { get; set; }
        public CommunicationWay CommunicationWay { get; set; }

        protected ConnectionOrientedConnector()
        {
            _logger = LogManager.GetLogger(this.GetType());
            CommunicationWay = CommunicationWay.Send;
            ConnectionState = ConnectionState.Disconnected;
            _sendLock = new object();
        }

        #region Control IRun Methods

        public override void Start()
        {
            if (ConnectionState != ConnectionState.Disconnected)
            {
                throw new Exception("Connector is already connected");
            }
            lock (_sendLock)
            {
                OnStateChange(ConnectionState.Connecting);
                StartCommunication();
                _logger.Debug($"{GetType().Name} started");
                OnStateChange(ConnectionState.Connected);
            }
        }

        public override void Stop()
        {
            lock (_sendLock)
            {
                OnStateChange(ConnectionState.Disconnecting);
                StopCommunication();
                _logger.Debug($"{GetType().Name} stoped");
                OnStateChange(ConnectionState.Disconnected);
            }
        }

        #endregion

        #region Listeners

        protected void OnStateChange(ConnectionState connectionState)
        {
            ConnectionState = connectionState;
            StateChanged?.Invoke(this, new ConnectorStateChangeEventArgs(this, connectionState, ConnectionState));
        }

        #endregion

        protected abstract void StartCommunication();

        protected abstract void StopCommunication();
        
        public override void SendMessage(Message message)
        {
            lock (_sendLock)
            {
                SendMessageInternal(message);                
            }
        }

        protected abstract void SendMessageInternal(Message message);
    }
}