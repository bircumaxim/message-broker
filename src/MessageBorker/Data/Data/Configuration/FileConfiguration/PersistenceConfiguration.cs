using System.Collections.Generic;
using System.Xml;
using Persistence.Configuration;
using Persistence.Models;
using Serialization.WireProtocol;

namespace Data.Configuration.FileConfiguration
{
    public class PersistenceConfiguration : IPersistenceConfiguration
    {
        private const string DefaultRootDirectory = ".\\messages";

        private readonly string _rootDirectory;
        private readonly IWireProtocol _wireProtocol;
        private readonly ExchangeAndQueuesConfiguration _exchangeAndQueuesConfiguration;
        
        public PersistenceConfiguration(XmlDocument configsDocument)
        {
            var fileConfigurationNode = configsDocument.SelectSingleNode("Broker/Persistence/FilePersistence");
            _rootDirectory =  fileConfigurationNode?.Attributes?.GetNamedItem("RootDirectory")?.Value ?? DefaultRootDirectory;
            _wireProtocol = WireProtocolConfigHelper.GetWireProtocolByName(fileConfigurationNode);
            _exchangeAndQueuesConfiguration = new ExchangeAndQueuesConfiguration(configsDocument);
        }

        public IWireProtocol GetPersistenceWireProtocol()
        {
            return _wireProtocol;
        }

        public string GetFilePersistenceRootDirectory()
        {
            return _rootDirectory;
        }

        public List<PersistenceExchange> GetExchangeDataList()
        {
            return _exchangeAndQueuesConfiguration.Exchanges;
        }

        public Dictionary<string, PersistenceQueue<PersistenceMessage>> GetQueueDataList()
        {
            return _exchangeAndQueuesConfiguration.Queues;
        }
    }
}