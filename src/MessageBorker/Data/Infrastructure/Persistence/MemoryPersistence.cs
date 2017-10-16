using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Persistence.Configuration;
using Persistence.Models;
using Persistence.Storages;
using Persistence.Storages.MemoryStorage;

namespace Persistence
{
    public class MemoryPersistence
    {
        private readonly ILog _logger;
        private readonly Storrage<List<PersistenceExchange>> _exchangesStorage;
        private readonly Storrage<Dictionary<string, PersistenceQueue<PersistenceMessage>>> _queuesStorrage;
        private readonly Storrage<PersistenceServerGeneralInfo> _serrverInfoStorrage;

        public MemoryPersistence(IPersistenceConfiguration configuration)
        {
            _logger = LogManager.GetLogger(GetType());
            _serrverInfoStorrage =
                MemoryStorageFactory.Instance.GetStorrageFor<PersistenceServerGeneralInfo>(typeof(PersistenceServerGeneralInfo));

            _queuesStorrage =
                MemoryStorageFactory.Instance.GetStorrageFor<Dictionary<string, PersistenceQueue<PersistenceMessage>>>(
                    typeof(Dictionary<string, PersistenceQueue<PersistenceMessage>>));
            _exchangesStorage =
                MemoryStorageFactory.Instance.GetStorrageFor<List<PersistenceExchange>>(typeof(List<PersistenceExchange>));

            _serrverInfoStorrage.Data = new PersistenceServerGeneralInfo {ServerStartTime = DateTime.Now};
            _queuesStorrage.Data = configuration.GetQueueDataList();
            _exchangesStorage.Data = configuration.GetExchangeDataList();
        }

        public void PersistMessage(string queueKey, PersistenceMessage message)
        {
            _queuesStorrage.Data[queueKey].Enqueue(message);
            _logger.Debug($"Saved {message.GetType().Name} to Queue with id=\"{queueKey}\"");
        }

        public PersistenceExchange GetExchangeByName(string exchangeName)
        {
            return _exchangesStorage.Data.FirstOrDefault(excnage => excnage.Name.Equals(exchangeName));
        }

        public PersistenceMessage GetMessageFromQueueWithName(string queueName)
        {
            return _queuesStorrage.Data[queueName].Dequeue();
        }

        public PersistenceServerGeneralInfo GetServerGeneralInfo()
        {
            _serrverInfoStorrage.Data.MessagesInQueue = _queuesStorrage.Data.Values
                .Where(queue => queue != null)
                .Sum(queue => queue.Count());
            return _serrverInfoStorrage.Data;
        }
    }
}