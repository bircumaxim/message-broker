﻿using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using MessageBuss.Broker;
using Serialization.WireProtocol;

namespace MessageBuss.Buss
{
    public static class BrockerFactory
    {
        public static BrokerClient GetBrocker(string brokerName, IPEndPoint endPoint, IPEndPoint receiverIpEndPoint,
            BrokerProtocolType socketBrokerProtocolType,
            string wireProtocol = "DefaultWireProtocol",
            bool isCryptingEnabled = false, Dictionary<string, string> defautlExchanges = null)
        {
            switch (socketBrokerProtocolType)
            {
                case BrokerProtocolType.Udp:
                    return new UdpBrokerClient(brokerName, GetWireProtocol(wireProtocol, isCryptingEnabled), endPoint,
                        receiverIpEndPoint,
                        defautlExchanges);
                case BrokerProtocolType.UdpMulticast:
                    return new UdpMulticastBrocker(brokerName, GetWireProtocol(wireProtocol, isCryptingEnabled),
                        endPoint, defautlExchanges);
                case BrokerProtocolType.Tcp:
                    return new TcpBrokerClient(brokerName, GetWireProtocol(wireProtocol, isCryptingEnabled), endPoint,
                        defautlExchanges);
            }
            return null;
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