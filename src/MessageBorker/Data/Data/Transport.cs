using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.GateWays;
using log4net;
using Transport;
using Transport.Events;

namespace Data
{
    public class Transport : ITransportGateWay
    {
        private readonly ILog _logger;
        private SortedList<int, RemoteApplication> _remoteApplications;
        private readonly SortedList<long, IConnector> _connectors;
        private readonly List<IConnectionManager> _connectionManagers;

        public Transport()
        {
            _logger = LogManager.GetLogger(GetType());
            _remoteApplications = new SortedList<int, RemoteApplication>();
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
                var communicatorIds = _connectors.Keys.ToArray();
                foreach (var communicatorId in communicatorIds)
                {
                    try
                    {
                        _connectors[communicatorId].Stop();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Failed to stpo connector wit id {communicatorId}");
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
            _logger.Debug($"New connector with id {args.Connector.ConnectorId} was connected");
        }
    }
}