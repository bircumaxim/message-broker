using System.Collections.Generic;
using System.IO;
using MessageBuss.Brocker;
using MessageBuss.Configuration;

namespace MessageBuss.Buss
{
    public class BussFactory
    {
        private static BussFactory _instance;
        public static BussFactory Instance => _instance ?? (_instance = new BussFactory());
        
        private static readonly string ConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "./config.xml");
        private readonly Dictionary<string, BrockerClient> _brockerClients;

        private BussFactory()
        {
            IConfiguration configuration = new FileConfiguration(ConfigFilePath);
            _brockerClients = configuration.GetBrockers();
        }

        public Buss GetBussFor(string brockerName)
        {
            var brocker = _brockerClients[brockerName];
            brocker.Start();
            return new Buss(brocker);
        }
    }
}