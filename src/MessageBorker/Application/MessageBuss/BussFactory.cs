using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using MessageBuss.Configuration;
using Messages;
using Serialization;
using Serialization.Serializers;
using Serialization.WireProtocols;
using Transport.Connectors.Tcp;
using Transport.Events;

namespace MessageBuss
{
    public class BussFactory
    {
        private static BussFactory _instance;
        public static BussFactory Instance => _instance ?? (_instance = new BussFactory());
        
        private static readonly string ConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "./config.xml");
        private readonly Dictionary<string, Brocker> _brockers;

        private BussFactory()
        {
            IConfiguration configuration = new FileConfiguration(ConfigFilePath);
            _brockers = configuration.GetBrockers();
        }

        public Buss GetBussFor(string brockerName)
        {
            var brocker = _brockers[brockerName];
            brocker.Start();
            return new Buss(brocker);
        }
    }
}