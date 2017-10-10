using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using MessageBuss.Brocker;
using Serialization;
using Serialization.WireProtocols;

namespace MessageBuss.Configuration
{
    internal class FileConfiguration : IConfiguration
    {
        private const string DefaultWireProtcolName = "DefaultWireProtocol";
        private const string DefaultIp = "127.0.0.1";
        private const string DefaultPort = "9000";
        private readonly Dictionary<string, BrockerClient> _brockerClients;

        internal FileConfiguration(string filePath)
        {
            _brockerClients = new Dictionary<string, BrockerClient>();
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
                    BrockerClient brockerClient = GetBrockerFromNode(brockerNode);
                    _brockerClients.Add(brockerClient.BrockerName, brockerClient);
                }
            }
        }

        private BrockerClient GetBrockerFromNode(XmlNode brockerNode)
        {
            BrockerClient brocker = null;
            if (brockerNode.Attributes != null)
            {
                var brockerName = brockerNode.Attributes.GetNamedItem("Name")?.Value;
                var wireProtocolName =
                    brockerNode.Attributes.GetNamedItem("WireProtocol")?.Value ?? DefaultWireProtcolName;
                var ip = brockerNode.Attributes.GetNamedItem("Ip")?.Value ?? DefaultIp;
                var port = Convert.ToInt32(brockerNode.Attributes.GetNamedItem("Port")?.Value ?? DefaultPort);
                var protocolType = brockerNode.Attributes.GetNamedItem("SocketProtocol")?.Value;
                brocker = GetBrockerBySocketProtocol(brockerName, GetWireProtocol(wireProtocolName),
                    new IPEndPoint(IPAddress.Parse(ip), port), GetDefaultExchanges(brockerNode), protocolType);
            }
            return brocker;
        }

        private BrockerClient GetBrockerBySocketProtocol(string brockerName, IWireProtocol wireProtocol,
            IPEndPoint endPoint, Dictionary<string, string> defautlExchanges, string socketProtocol)
        {
            switch (socketProtocol)
            {
                case "Udp":
                    return new UdpBrockerClient(brockerName, wireProtocol, endPoint, defautlExchanges);
                default:
                    return new TcpBrockerClient(brockerName, wireProtocol, endPoint, defautlExchanges);
            }
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


        public Dictionary<string, BrockerClient> GetBrockers()
        {
            return _brockerClients;
        }
    }
}