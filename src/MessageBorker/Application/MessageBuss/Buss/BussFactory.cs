using System.Collections.Generic;
using System.IO;
using MessageBuss.Broker;
using MessageBuss.Configuration;

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
    }
}