using System.Collections.Generic;
using Persistence.Models;
using Serialization.WireProtocol;

namespace Persistence.Configuration
{
    public interface IPersistenceConfiguration
    {
        IWireProtocol GetPersistenceWireProtocol();
        string GetFilePersistenceRootDirectory();
        List<PersistenceExchange> GetExchangeDataList();
        Dictionary<string, PersistenceQueue<PersistenceMessage>> GetQueueDataList();
    }
}