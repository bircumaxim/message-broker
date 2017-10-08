using System.Collections.Generic;
using Data.Models;
using Transport;

namespace Data.Configuration
{
    public interface IConfiguration
    {
        List<IConnectionManager> GetConnectionManagers();
        List<ExchangeData> GetExchangeDataList();
        Dictionary<string, QueueData<MessageData>> GetQueueDataList();
    }
}