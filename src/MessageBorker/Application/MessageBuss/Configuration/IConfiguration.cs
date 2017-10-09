using System.Collections.Generic;
using MessageBuss.Brocker;

namespace MessageBuss.Configuration
{
    internal interface IConfiguration
    {
        Dictionary<string, BrockerClient> GetBrockers();
    }
}