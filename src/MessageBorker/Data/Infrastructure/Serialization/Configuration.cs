using System.IO;
using System.Xml;

namespace Serialization
{
    public class Configuration
    {
        private static Configuration _instance;
        public static Configuration Instance => _instance ?? (_instance = new Configuration());
        
        private static readonly string ConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/Infrastructure/Serialization/config.xml");
        public string ObjectsToSerializeModule { get; set; }

        public Configuration()
        {
            LoadConfigsFromFile();
        }

        private void LoadConfigsFromFile()
        {
            var configsDocument = new XmlDocument();
            configsDocument.Load(ConfigFilePath);
            ObjectsToSerializeModule = configsDocument
                .SelectSingleNode("/Serialization/ObjectsToSerializeModule")
                ?.Attributes
                ?.GetNamedItem("Name").Value;
        }
    }
}