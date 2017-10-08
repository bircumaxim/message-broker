using System.Collections.Generic;
using Serialization;

namespace MessageBuss.Configuration
{
    public interface IConfiguration
    {
        Dictionary<string,Brocker> GetBrockers();
    }
}