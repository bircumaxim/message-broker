using System.Collections.Generic;
using System.Xml;
using Data.Models;
using Persistence.Configuration;
using Serialization.WireProtocol;
using Transport;

namespace Data.Configuration.FileConfiguration
{
    public class FileConfiguration : IConfiguration
    {
        private readonly ConnectionManagersConfiguration _connectionManagersConfiguration;
        private readonly PersistenceConfiguration _persistenceConfiguration;

        public FileConfiguration(string filePath)
        {
            var configsXmlDocument = new XmlDocument();
            configsXmlDocument.Load(filePath);
            _connectionManagersConfiguration = new ConnectionManagersConfiguration(configsXmlDocument); 
            _persistenceConfiguration = new PersistenceConfiguration(configsXmlDocument);
        }

        public List<IConnectionManager> GetConnectionManagers()
        {
            return _connectionManagersConfiguration.ConnectionManagers;
        }

        public IPersistenceConfiguration GetPersistenceConfiguration()
        {
            return _persistenceConfiguration;
        }
    }
}