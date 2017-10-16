using System.Collections.Generic;

namespace Persistence.Models
{
    public class PersistenceExchange
    {
        public string Name { get; set; }
        public PersistenceExchangeType ExchangeType { get; set; }
        public Dictionary<string, PersistenceQueue<PersistenceMessage>> Queues { get; set; }
    }
}