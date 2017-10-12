using System;
using System.Collections.Generic;
using System.Linq;
using Data.Configuration;
using Data.Models;
using log4net;
using Persistence.Storages;
using Persistence.Storages.MemoryStorage;

namespace Data
{
    public class MemoryPersistence
    {
        private readonly ILog _logger;
        private readonly Storrage<List<ExchangeData>> _exchangesStorage;
        private readonly Storrage<Dictionary<string, QueueData<MessageData>>> _queuesStorrage;
        private readonly Storrage<ServerGeneralInfo> _serrverInfoStorrage;

        public MemoryPersistence(IConfiguration configuration)
        {
            _logger = LogManager.GetLogger(GetType());
            _serrverInfoStorrage =
                MemoryStorageFactory.Instance.GetStorrageFor<ServerGeneralInfo>(typeof(ServerGeneralInfo));

            _queuesStorrage =
                MemoryStorageFactory.Instance.GetStorrageFor<Dictionary<string, QueueData<MessageData>>>(
                    typeof(Dictionary<string, QueueData<MessageData>>));
            _exchangesStorage =
                MemoryStorageFactory.Instance.GetStorrageFor<List<ExchangeData>>(typeof(List<ExchangeData>));

            _serrverInfoStorrage.Data = new ServerGeneralInfo {ServerStartTime = DateTime.Now};
            _queuesStorrage.Data = configuration.GetQueueDataList();
            _exchangesStorage.Data = configuration.GetExchangeDataList();
        }

        public void PersistMessage(string queueKey, MessageData message)
        {
            _queuesStorrage.Data[queueKey].Enqueue(message);
            _logger.Debug($"Saved {message.GetType().Name} to Queue with id=\"{queueKey}\"");
        }

        public ExchangeData GetExchangeByName(string exchangeName)
        {
            return _exchangesStorage.Data.FirstOrDefault(excnage => excnage.Name.Equals(exchangeName));
        }

        public MessageData GetMessageFromQueueWithName(string queueName)
        {
            return _queuesStorrage.Data[queueName].Dequeue();
        }

        public ServerGeneralInfo GetServerGeneralInfo()
        {
            _serrverInfoStorrage.Data.MessagesInQueue = _queuesStorrage.Data.Values
                .Where(queue => queue != null)
                .Sum(queue => queue.Count());
            return _serrverInfoStorrage.Data;
        }
    }
}