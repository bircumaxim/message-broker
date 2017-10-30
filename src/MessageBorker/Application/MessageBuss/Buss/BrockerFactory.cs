using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using MessageBuss.Broker;
using Serialization.WireProtocol;

namespace MessageBuss.Buss
{
    public static class BrockerFactory
    {
        public static BrokerClient GetBrocker(string brokerName, IPEndPoint endPoint, string socketProtocol = "Udp",
            string wireProtocol = "DefaultWireProtocol",
            bool isCryptingEnabled = false, Dictionary<string, string> defautlExchanges = null)
        {
            switch (socketProtocol)
            {
                case "Udp":
                    return new UdpBrokerClient(brokerName, GetWireProtocol(wireProtocol, isCryptingEnabled), endPoint,
                        defautlExchanges);
                case "UdpMulticast":
                    return new UdpMulticastBrocker(brokerName, GetWireProtocol(wireProtocol, isCryptingEnabled),
                        endPoint, defautlExchanges);
                default:
                    return new TcpBrokerClient(brokerName, GetWireProtocol(wireProtocol, isCryptingEnabled), endPoint,
                        defautlExchanges);
            }
        }

        private static IWireProtocol GetWireProtocol(string wireProtocolName, bool isCryptingEnabled)
        {
            switch (wireProtocolName)
            {
                default:
                    return new DefaultWireProtocol(isCryptingEnabled);
            }
        }
    }
}