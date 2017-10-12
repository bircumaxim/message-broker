using System.Collections.Generic;
using System.Xml;
using Data.Models;
using Serialization.WireProtocol;
using Transport;

namespace Data.Configuration.FileConfiguration
{
    public class FileConfiguration : IConfiguration
    {
        private readonly ConnectionManagersConfiguration _connectionManagersConfiguration;
        private readonly ExchangeAndQueuesConfiguration _exchangeAndQueuesConfiguration;
        private readonly PersistenceConfiguration _persistenceConfiguration;

        public FileConfiguration(string filePath)
        {
            var configsXmlDocument = new XmlDocument();
            configsXmlDocument.Load(filePath);
            _connectionManagersConfiguration = new ConnectionManagersConfiguration(configsXmlDocument);
            _exchangeAndQueuesConfiguration = new ExchangeAndQueuesConfiguration(configsXmlDocument); 
            _persistenceConfiguration = new PersistenceConfiguration(configsXmlDocument);
        }

        public List<IConnectionManager> GetConnectionManagers()
        {
            return _connectionManagersConfiguration.ConnectionManagers;
        }

        public List<ExchangeData> GetExchangeDataList()
        {
            return _exchangeAndQueuesConfiguration.Exchanges;
        }

        public Dictionary<string, QueueData<MessageData>> GetQueueDataList()
        {
            return _exchangeAndQueuesConfiguration.Queues;
        }

        public IWireProtocol GetPersistenceWireProtocol()
        {
            return _persistenceConfiguration.WireProtocol;
        }

        public string GetFilePersistenceRootDirectory()
        {
            return _persistenceConfiguration.RootDirectory;
        }
    }
}