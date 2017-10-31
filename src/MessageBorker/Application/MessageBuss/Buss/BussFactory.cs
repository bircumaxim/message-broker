using System.Collections.Generic;
using System.IO;
using System.Net;
using MessageBuss.Broker;
using MessageBuss.Configuration;
using Messages.Connection;

namespace MessageBuss.Buss
{
    public class BussFactory
    {
        private static BussFactory _instance;
        public static BussFactory Instance => _instance ?? (_instance = new BussFactory());

        private static readonly string ConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "./config.xml");
        private readonly Dictionary<string, BrokerClient> _brokerClients;

        private BussFactory()
        {
            IConfiguration configuration = new FileConfiguration(ConfigFilePath);
            _brokerClients = configuration.GetBrokers();
        }

        public Buss GetBussFor(string brokerName)
        {
            var broker = _brokerClients[brokerName];
            broker.StartAsync();
            return new Buss(broker);
        }

        public Buss GetBussFor(string brockerName, IPEndPoint brockerIpEndPoint, IPEndPoint receiverIpEndPoint,
            string protcolType = "Udp")
        {
            if (!_brokerClients.ContainsKey(brockerName))
            {
                var brokerClient =
                    BrockerFactory.GetBrocker(brockerName, brockerIpEndPoint, receiverIpEndPoint, protcolType);
                _brokerClients.Add(brockerName, brokerClient);
            }

            var broker = _brokerClients[brockerName];
            broker.StartAsync();
            return new Buss(broker);
        }
    }
}