using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using MessageBuss.Broker;
using Serialization;
using Serialization.WireProtocol;

namespace MessageBuss.Configuration
{
    internal class FileConfiguration : IConfiguration
    {
        private const string DefaultWireProtcolName = "DefaultWireProtocol";
        private const string DefaultIp = "127.0.0.1";
        private const string DefaultPort = "9000";
        private readonly Dictionary<string, BrokerClient> _brokerClients;

        internal FileConfiguration(string filePath)
        {
            _brokerClients = new Dictionary<string, BrokerClient>();
            var configsDocument = new XmlDocument();
            configsDocument.Load(filePath);
            LoadConfigurationFrom(configsDocument);
        }

        private void LoadConfigurationFrom(XmlDocument configsDocument)
        {
            var brokerNodes = configsDocument.SelectSingleNode("/Buss/Brokers");
            if (brokerNodes != null)
            {
                foreach (XmlElement brokerNode in brokerNodes)
                {
                    BrokerClient brokerClient = GetBrokerFromNode(brokerNode);
                    _brokerClients.Add(brokerClient.BrokerName, brokerClient);
                }
            }
        }

        private BrokerClient GetBrokerFromNode(XmlNode brokerNode)
        {
            BrokerClient broker = null;
            if (brokerNode.Attributes != null)
            {
                var brokerName = brokerNode.Attributes.GetNamedItem("Name")?.Value;
                var wireProtocolName =
                    brokerNode.Attributes.GetNamedItem("WireProtocol")?.Value ?? DefaultWireProtcolName;
                var ip = brokerNode.Attributes.GetNamedItem("Ip")?.Value ?? DefaultIp;
                var port = Convert.ToInt32(brokerNode.Attributes.GetNamedItem("Port")?.Value ?? DefaultPort);
                var protocolType = brokerNode.Attributes.GetNamedItem("SocketProtocol")?.Value;
                var enableCrypting = Convert.ToBoolean(brokerNode.Attributes.GetNamedItem("EnableCrypting")?.Value);
                broker = GetBrokerBySocketProtocol(brokerName, GetWireProtocol(wireProtocolName, enableCrypting),
                    new IPEndPoint(IPAddress.Parse(ip), port), GetDefaultExchanges(brokerNode), protocolType);
            }
            return broker;
        }

        private BrokerClient GetBrokerBySocketProtocol(string brokerName, IWireProtocol wireProtocol,
            IPEndPoint endPoint, Dictionary<string, string> defautlExchanges, string socketProtocol)
        {
            switch (socketProtocol)
            {
                case "Udp":
                    return new UdpBrokerClient(brokerName, wireProtocol, endPoint, defautlExchanges);
                default:
                    return new TcpBrokerClient(brokerName, wireProtocol, endPoint, defautlExchanges);
            }
        }

        private Dictionary<string, string> GetDefaultExchanges(XmlNode brokerNode)
        {
            var defaultExchanges = new Dictionary<string, string>();
            foreach (XmlElement exchangeNode in brokerNode)
            {
                var exchangeType = exchangeNode.Name;
                var exchangeName = exchangeNode.Attributes.GetNamedItem("Name").Value;
                defaultExchanges.Add(exchangeType, exchangeName);
            }
            return defaultExchanges;
        }

        private IWireProtocol GetWireProtocol(string wireProtocolName, bool isCryptingEnabled)
        {
            switch (wireProtocolName)
            {
                default:
                    return new DefaultWireProtocol(isCryptingEnabled);
            }
        }

        public Dictionary<string, BrokerClient> GetBrokers()
        {
            return _brokerClients;
        }
    }
}