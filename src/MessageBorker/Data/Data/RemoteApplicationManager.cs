using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Data.Configuration;
using Data.Events;
using log4net;
using Messages;
using Messages.Connection;
using Serialization;
using Transport;
using Transport.Connectors.Udp;
using Transport.Events;

namespace Data
{
    public class RemoteApplicationManager : IRun
    {
        private readonly List<IConnectionManager> _connectionManagers;
        private readonly Dictionary<string, RemoteApplication> _remoteApplications;
        public event RemoteApplicationMessageReceived RemoteApplicationMessageReceived;

        public RemoteApplicationManager(IConfiguration configuration)
        {
            _remoteApplications = new Dictionary<string, RemoteApplication>();
            _connectionManagers = configuration.GetConnectionManagers();
        }

        #region IRun methods

        public void Start()
        {
            _connectionManagers.ForEach(manager =>
            {
                manager.ConnectorConnected += OnConnectorConnected;
                manager.Start();
            });
        }

        public Task StartAsync()
        {
            return Task.Factory.StartNew(() =>
                _connectionManagers.ForEach(manager =>
                {
                    manager.ConnectorConnected += OnConnectorConnected;
                    manager.StartAsync();
                })
            );
        }

        public void Stop()
        {
            SendConnectionCloseMessageToRemoteApplicatoins();
            _connectionManagers.ForEach(manager =>
            {
                manager.Stop();
                manager.ConnectorConnected -= OnConnectorConnected;
            });
            StopRemoteApplications();
        }

        private void SendConnectionCloseMessageToRemoteApplicatoins()
        {
            lock (_remoteApplications)
            {
                foreach (var remoteApplication in _remoteApplications.Values)
                {
                    remoteApplication.Send(new CloseConnectionResponse());
                }   
            }
        }

        #endregion

        #region Listeners

        private void OnConnectorConnected(object sender, ConnectorConnectedEventArgs args)
        {
            var connector = args.Connector;
            connector.MessageReceived += OnMessageReceivedFromConnector;
            connector.StartAsync();
        }

        private void OnMessageReceivedFromConnector(object seneder, MessageReceivedEventArgs args)
        {
            if (args.Message.MessageTypeName == typeof(OpenConnectionRequest).Name)
            {
                var remoteApplication = new RemoteApplication(args.Connector);
                args.Connector.MessageReceived -= OnMessageReceivedFromConnector;
                remoteApplication.RemoteApplicationMessageReceived += RemoteApplicationMessageReceived;
                AddRemoteApplication(remoteApplication);
                remoteApplication.Send(new OpenConnectionResponse
                {
                    IsConnectionAccepted = true,
                    ClientName = args.Connector.ConnectorId
                });
            }
        }

        public void AddRemoteApplication(RemoteApplication remoteApplication)
        {
            lock (_remoteApplications)
            {
                _remoteApplications.Add(remoteApplication.Name, remoteApplication);
            }
        }

        #endregion

        private void StopRemoteApplications()
        {
            lock (_remoteApplications)
            {
                foreach (var remoteApplication in _remoteApplications.Values)
                {
                    remoteApplication.RemoteApplicationMessageReceived -= RemoteApplicationMessageReceived;
                    remoteApplication.Stop();
                }
                _remoteApplications.Clear();
            }
        }
        
        public int GetConnectionsNumber()
        {
            return _remoteApplications.Count;
        }
        
        public void SendMessage(string applicationName, Message message)
        {
            lock (_remoteApplications)
            {
                _remoteApplications[applicationName].Send(message);
            }
        }
        
        public void StopRemoteApplication(string applicationName)
        {
            lock (_remoteApplications)
            {
                _remoteApplications[applicationName].Stop();
                _remoteApplications[applicationName].RemoteApplicationMessageReceived -= RemoteApplicationMessageReceived;
                _remoteApplications.Remove(applicationName);
            }
        }
    }
}