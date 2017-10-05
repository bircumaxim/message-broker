using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Domain.GateWays;
using log4net;
using Transport;
using Transport.Connectors;
using Transport.Events;

namespace Data
{
    public class Transport : ITransportGateWay
    {
        private readonly ILog _logger;
        private readonly Dictionary<string, RemoteApplication> _remoteApplications;
        private readonly SortedList<long, IConnector> _connectors;
        private readonly List<IConnectionManager> _connectionManagers;

        public Transport()
        {
            _logger = LogManager.GetLogger(GetType());
            _remoteApplications = new Dictionary<string, RemoteApplication>();
            _connectors = new SortedList<long, IConnector>();
            _connectionManagers = new List<IConnectionManager>();
            AddConnectionManager(new TcpConnectionManager(Convert.ToInt32(9000)));
        }

        public void Start()
        {
            foreach (var manager in _connectionManagers)
            {
                manager.Start();
            }
        }

        public Task StartAsync()
        {
            return Task.Factory.StartNew(() => _connectionManagers.ForEach(manager => manager.StartAsync()));
        }

        public void Stop()
        {
            _connectionManagers.ForEach(manager => manager.Stop());
            StopConnectors();
            ClearConnectors();
        }

        private void StopConnectors()
        {
            lock (_connectors)
            {
                foreach (var connectorId in _connectors.Keys.ToArray())
                {
                    try
                    {
                        _connectors[connectorId].Stop();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to stpo connector wit id {connectorId}");
                    }
                }
            }
        }

        private void ClearConnectors()
        {
            lock (_connectors)
            {
                _connectors.Clear();
            }
        }
        
        private void AddConnectionManager(IConnectionManager connectionManager)
        {
            connectionManager.ConnectorConnected += OnConnectorConnected;
            _connectionManagers.Add(connectionManager);
        }

        private void OnConnectorConnected(object sender, ConnectorConnectedEventArgs args)
        {
            var remoteApplication = new RemoteApplication(args.Connector);
            var applicationId = remoteApplication.Name;
            _remoteApplications.Add(applicationId, remoteApplication);
            remoteApplication.StartAsync();
        }
    }
}