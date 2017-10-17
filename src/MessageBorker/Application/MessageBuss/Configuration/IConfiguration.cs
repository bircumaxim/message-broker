using System.Collections.Generic;
using MessageBuss.Broker;

namespace MessageBuss.Configuration
{
    internal interface IConfiguration
    {
        Dictionary<string, BrokerClient> GetBrokers();
    }
}