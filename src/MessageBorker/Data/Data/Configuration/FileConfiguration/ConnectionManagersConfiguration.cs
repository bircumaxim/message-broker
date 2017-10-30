using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Xml;
using Serialization.WireProtocol;
using Transport;

namespace Data.Configuration.FileConfiguration
{
    public class ConnectionManagersConfiguration
    {
        public List<IConnectionManager> ConnectionManagers { get; }

        private const string DefaultPort = "9000";
        private const string DefaultIp = "224.5.6.7";
        private const string DefaultMessageLength = "52428800";

        public ConnectionManagersConfiguration(XmlDocument configsDocument)
        {
            ConnectionManagers = new List<IConnectionManager>();
            LoadConnectionManagers(configsDocument);
        }

        private void LoadConnectionManagers(XmlDocument configsDocument)
        {
            var connectionManagers = configsDocument.SelectSingleNode("/Broker/ConnectionManagers");
            if (connectionManagers == null) return;
            foreach (XmlElement connectionManagerXmlElement in connectionManagers)
            {
                var managerName = connectionManagerXmlElement.Name;
                if (managerName == typeof(UdpConnectionManager).Name)
                    AddUdpConnectionManager(connectionManagerXmlElement);
                else if (managerName == typeof(TcpConnectionManager).Name)
                {
                    AddTcpConnectionManager(connectionManagerXmlElement);
                }
                else if (managerName == typeof(UdpMulticastConnectionManager).Name)
                {
                    AddUdpMulticastConnectionManager(connectionManagerXmlElement);
                }
            }
        }

        private void AddUdpConnectionManager(XmlNode connectionManager)
        {
            if (connectionManager.Attributes != null)
            {
                var port = Convert.ToInt32(connectionManager.Attributes.GetNamedItem("Port")?.Value ?? DefaultPort);
                ConnectionManagers.Add(new UdpConnectionManager(port,
                    WireProtocolConfigHelper.GetWireProtocolByName(connectionManager)));
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
                ConnectionManagers.Add(new TcpConnectionManager(port,
                    WireProtocolConfigHelper.GetWireProtocolByName(connectionManager),
                    maxMessageLength));
            }
        }

        private void AddUdpMulticastConnectionManager(XmlNode connectionManager)
        {
            if (connectionManager.Attributes != null)
            {
                var ip = connectionManager.Attributes.GetNamedItem("Ip")?.Value ?? DefaultIp;
                var port = Convert.ToInt32(connectionManager.Attributes.GetNamedItem("Port")?.Value ?? DefaultPort);
                var maxMessageLength = Convert.ToInt32(
                    connectionManager.Attributes.GetNamedItem("MaxMessageLength")?.Value ??
                    DefaultMessageLength);
                ConnectionManagers.Add(new UdpMulticastConnectionManager(new IPEndPoint(IPAddress.Parse(ip), port),
                    WireProtocolConfigHelper.GetWireProtocolByName(connectionManager),
                    maxMessageLength,
                    getQueuesTosubscribe(connectionManager)));
            }
        }

        private List<string> getQueuesTosubscribe(XmlNode connectionManager)
        {
            List<string> queues = new List<string>();

            foreach (XmlElement queueXmlElement in connectionManager)
            {
                var queueName = queueXmlElement.Attributes.GetNamedItem("Name").Value;
                if (!string.IsNullOrEmpty(queueName))
                {
                    queues.Add(queueName);
                }
            }
            return queues;
        }
    }
}