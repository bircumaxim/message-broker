using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using Serialization;
using Serialization.WireProtocols;

namespace MessageBuss.Configuration
{
    public class FileConfiguration : IConfiguration
    {
        private const string DefaultWireProtcolName = "DefaultWireProtocol";
        private const string DefaultIp = "127.0.0.1";
        private const string DefaultPort = "9000";
        private readonly Dictionary<string, Brocker> _brockers;

        public FileConfiguration(string filePath)
        {
            _brockers = new Dictionary<string, Brocker>();
            var configsDocument = new XmlDocument();
            configsDocument.Load(filePath);
            LoadConfigurationFrom(configsDocument);
        }

        private void LoadConfigurationFrom(XmlDocument configsDocument)
        {
            var brockerNodes = configsDocument.SelectSingleNode("/Buss/Brockers");
            if (brockerNodes != null)
            {
                foreach (XmlElement brockerNode in brockerNodes)
                {
                    Brocker brocker = GetBrockerFromNode(brockerNode);
                    _brockers.Add(brocker.Name, brocker);
                }
            }
        }

        private Brocker GetBrockerFromNode(XmlNode brockerNode)
        {
            Brocker brocker = null;
            if (brockerNode.Attributes != null)
            {
                var brockerName = brockerNode.Attributes.GetNamedItem("Name")?.Value;
                var wireProtocolName =
                    brockerNode.Attributes.GetNamedItem("WireProtocol")?.Value ?? DefaultWireProtcolName;
                var ip = brockerNode.Attributes.GetNamedItem("Ip")?.Value ?? DefaultIp;
                var port = Convert.ToInt32(brockerNode.Attributes.GetNamedItem("Port")?.Value ?? DefaultPort);

                brocker = new Brocker(brockerName, GetWireProtocol(wireProtocolName),
                    new IPEndPoint(IPAddress.Parse(ip), port), GetDefaultExchanges(brockerNode));
            }
            return brocker;
        }

        private Dictionary<string, string> GetDefaultExchanges(XmlNode brockerNode)
        {
            var defaultExchanges = new Dictionary<string, string>();
            foreach (XmlElement exchangeNode in brockerNode)
            {
                var exchangeType = exchangeNode.Name;
                var exchangeName = exchangeNode.Attributes.GetNamedItem("Name").Value;
                defaultExchanges.Add(exchangeType, exchangeName);   
            }
            return defaultExchanges;
        }

        private IWireProtocol GetWireProtocol(string wireProtocolName)
        {
            switch (wireProtocolName)
            {
                default:
                    return new DefaultWireProtocol();
            }
        }


        public Dictionary<string, Brocker> GetBrockers()
        {
            return _brockers;
        }
    }
}