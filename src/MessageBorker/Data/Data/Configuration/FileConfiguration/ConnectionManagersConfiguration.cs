using System;
using System.Collections.Generic;
using System.Xml;
using Serialization;
using Serialization.WireProtocols;
using Transport;

namespace Data.Configuration.FileConfiguration
{
    public class ConnectionManagersConfiguration
    {
        public List<IConnectionManager> ConnectionManagers { get; }

        private const string DefaultPort = "9000";
        private const string DefaultMessageLength = "52428800";

        public ConnectionManagersConfiguration(XmlDocument configsDocument)
        {
            ConnectionManagers = new List<IConnectionManager>();
            LoadConnectionManagers(configsDocument);
        }

        private void LoadConnectionManagers(XmlDocument configsDocument)
        {
            var connectionManagers = configsDocument.SelectSingleNode("/MessageBrocker/ConnectionManagers");
            if (connectionManagers == null) return;
            foreach (XmlElement connectionManagerXmlElement in connectionManagers)
            {
                switch (connectionManagerXmlElement.Name)
                {
                    case "UdpConnectionManager":
                        AddUdpConnectionManager(connectionManagerXmlElement);
                        break;
                    case "TcpConnectionManager":
                        AddTcpConnectionManager(connectionManagerXmlElement);
                        break;
                }
            }
        }

        private void AddUdpConnectionManager(XmlNode connectionManager)
        {
            if (connectionManager.Attributes != null)
            {
                var port = Convert.ToInt32(connectionManager.Attributes.GetNamedItem("Port")?.Value ?? DefaultPort);
                ConnectionManagers.Add(new UdpConnectionManager(port, GetWireProtocolByName(connectionManager)));
            }
        }

        private void AddTcpConnectionManager(XmlNode connectionManager)
        {
            if (connectionManager.Attributes != null)
            {
                var port = Convert.ToInt32(connectionManager.Attributes.GetNamedItem("Port")?.Value ?? DefaultPort);
                var maxMessageLength = Convert.ToInt32(
                    connectionManager.Attributes.GetNamedItem("MaxMessageLength")?.Value ??
                    DefaultMessageLength);
                ConnectionManagers.Add(new TcpConnectionManager(port, GetWireProtocolByName(connectionManager),
                    maxMessageLength));
            }
        }

        private IWireProtocol GetWireProtocolByName(XmlNode connectionManager)
        {
            IWireProtocol wireProtocol = null;
            if (connectionManager.Attributes != null)
            {
                var wireProtocolName = connectionManager.Attributes.GetNamedItem("WireProtocol")?.Value;
                switch (wireProtocolName)
                {
                    default:
                        wireProtocol = new DefaultWireProtocol();
                        break;
                }
            }
            return wireProtocol;
        }
    }
}