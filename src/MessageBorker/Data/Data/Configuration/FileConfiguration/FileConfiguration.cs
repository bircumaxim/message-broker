using System.Collections.Generic;
using System.Xml;
using Data.Models;
using Transport;

namespace Data.Configuration.FileConfiguration
{
    public class FileConfiguration : IConfiguration
    {
        private readonly ConnectionManagersConfiguration _connectionManagersConfiguration;
        private readonly ExchangeAndQueuesConfiguration _exchangeAndQueuesConfiguration;

        public FileConfiguration(string filePath)
        {
            var configsDocument = new XmlDocument();
            configsDocument.Load(filePath);
            _connectionManagersConfiguration = new ConnectionManagersConfiguration(configsDocument);
            _exchangeAndQueuesConfiguration = new ExchangeAndQueuesConfiguration(configsDocument);
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
    }
}