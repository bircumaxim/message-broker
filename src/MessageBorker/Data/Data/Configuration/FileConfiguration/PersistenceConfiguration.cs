using System.Xml;
using Serialization.WireProtocol;

namespace Data.Configuration.FileConfiguration
{
    public class PersistenceConfiguration
    {
        private const string DefaultRootDirectory = ".\\messages";
        
        public string RootDirectory { get; set; }
        public IWireProtocol WireProtocol { get; set; }
        
        public PersistenceConfiguration(XmlDocument configsDocument)
        {
            var fileConfigurationNode = configsDocument.SelectSingleNode("MessageBrocker/Persistence/FilePersistence");
            LoadFileConfigurations(fileConfigurationNode);
            WireProtocol = WireProtocolConfigHelper.GetWireProtocolByName(fileConfigurationNode);
        }

        private void LoadFileConfigurations(XmlNode configurationNode)
        {
            RootDirectory =  configurationNode?.Attributes?.GetNamedItem("RootDirectory")?.Value ?? DefaultRootDirectory;
        }
    }
}