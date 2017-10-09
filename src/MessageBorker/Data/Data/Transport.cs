using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Data.Configuration;
using Data.Events;
using Data.Mappers;
using Data.Models;
using Domain.GateWays;
using Domain.UseCases;
using log4net;
using Transport;
using Transport.Connectors;
using Transport.Events;

namespace Data
{
    public class Transport : ITransportGateWay
    {
        private readonly Dictionary<string, RemoteApplication> _remoteApplications;
        private readonly List<IConnectionManager> _connectionManagers;
        private readonly UseCaseFactory _useCaseFactory;

        public Transport(IConfiguration configuration)
        {
            var persistence = new Persistence(configuration);
            _useCaseFactory = new UseCaseFactory(persistence);
            _remoteApplications = new Dictionary<string, RemoteApplication>();
            _connectionManagers = configuration.GetConnectionManagers();
        }

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
            _connectionManagers.ForEach(manager =>
            {
                manager.Stop();
                manager.ConnectorConnected -= OnConnectorConnected;
            });
            StopRemoteApplications();
        }

        private void StopRemoteApplications()
        {
            lock (_remoteApplications)
            {
                foreach (var remoteApplication in _remoteApplications.Values)
                {
                    remoteApplication.MessageReceivedFromRemoteApplication -= OnMessageReceivedFromRemoteApplication;
                    remoteApplication.Stop();
                }
                _remoteApplications.Clear();
            }
        }

        private void OnConnectorConnected(object sender, ConnectorConnectedEventArgs args)
        {
            var connector = args.Connector;
            connector.MessageReceived += OnMessageReceivedFromConnector;
            connector.StartAsync();
        }

        private void OnMessageReceivedFromConnector(object seneder, MessageReceivedEventArgs args)
        {
            if (args.Message.MessageTypeName == "OpenConnectionMessage")
            {
                var remoteApplication = new RemoteApplication(args.Connector);
                remoteApplication.MessageReceivedFromRemoteApplication += OnMessageReceivedFromRemoteApplication;
                lock (_remoteApplications)
                {
                    _remoteApplications.Add(remoteApplication.Name, remoteApplication);
                }
                remoteApplication.StartAsync();
            }
        }

        private void OnMessageReceivedFromRemoteApplication(object seneder,
            MessageReceivedFromRemoteApplicationEventArgs args)
        {
            switch (args.Message.MessageTypeName)
            {
                case "CloseConnectionMessage":
                    RemoveApplication(args.Application.Name);
                    break;
                case "PingMessage":
                    SendPongMessage(args.Application.Name);
                    break;
                case "PongMessage":
                    //TODO add logs here.
                    break;
                default:
                    var useCase = _useCaseFactory.GetUseCaseFor(args.Message);
                    useCase?.Execute();
                    break;
            }
        }

        private void SendPongMessage(string applicationName)
        {
            //TODO send here pong message to remote app.
        }

        private void RemoveApplication(string applicationName)
        {
            lock (_remoteApplications)
            {
                _remoteApplications[applicationName].Stop();
                _remoteApplications[applicationName].MessageReceivedFromRemoteApplication -=
                    OnMessageReceivedFromRemoteApplication;
                _remoteApplications.Remove(applicationName);
            }
        }
    }
}