using System.Collections.Generic;
using Data.Models;
using Domain.Exhcanges;
using Domain.GateWays;
using Persistence.Storages;

namespace Data
{
    public class Persistence : IPersistenceGateWay
    {
        private readonly Storrage<ExchangeData> _exchangeDataStorage;
        
        public Persistence()
        {
            _exchangeDataStorage = MemoryStorageFactory.Instance.GetStorrageFor<ExchangeData>(typeof(ExchangeData));
        }

        public List<Exchange> GetExchanges()
        {
            return null;
        }
    }
}