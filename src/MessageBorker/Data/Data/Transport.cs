using System;
using System.Collections.Generic;
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
            var remoteApplication = new RemoteApplication(args.Connector);
            remoteApplication.MessageReceivedFromRemoteApplication += OnMessageReceivedFromRemoteApplication;
            lock (_remoteApplications)
            {
                _remoteApplications.Add(remoteApplication.Name, remoteApplication);
            }
            remoteApplication.StartAsync();
        }

        private void OnMessageReceivedFromRemoteApplication(object seneder, MessageReceivedFromRemoteApplicationEventArgs args)
        {
            var useCase = _useCaseFactory.GetUseCaseFor(args.Message);
            useCase?.Execute();
        }
    }
}