using System.Collections.Generic;
using System.Linq;
using Data.Configuration;
using Data.Mappers;
using Data.Models;
using Domain.Exhcanges;
using Domain.GateWays;
using Persistence.Storages;
using Serialization;
using Message = Domain.Messages.Message;

namespace Data
{
    public class Persistence : IPersistenceGateWay
    {
        private readonly MessageMapper _messageMapper;
        private readonly ExchangeMapper _exchangeMapper;
        private readonly Storrage<List<ExchangeData>> _exchangeDataStorage;
        private readonly Storrage<Dictionary<string, QueueData<MessageData>>> _queuesData;
        private readonly IConfiguration _configuration;

        public Persistence(IConfiguration configuration)
        {
            _messageMapper = new MessageMapper();
            _exchangeMapper = new ExchangeMapper();
            _configuration = configuration;

            _queuesData = MemoryStorageFactory.Instance
                .GetStorrageFor<Dictionary<string, QueueData<MessageData>>>(
                    typeof(Dictionary<string, QueueData<MessageData>>));
            _exchangeDataStorage =
                MemoryStorageFactory.Instance.GetStorrageFor<List<ExchangeData>>(typeof(List<ExchangeData>));
            InitStorages();
        }

        private void InitStorages()
        {
            _queuesData.Data = _configuration.GetQueueDataList();
            _exchangeDataStorage.Data = _configuration.GetExchangeDataList();
        }

        public void PersistQueues(Dictionary<string, Domain.Queue<Message>> queues)
        {
            foreach (var queue in queues)
            {
                var queueData = _queuesData.Data[queue.Key];
                while (!queue.Value.IsEmpty())
                {
                    var message = queue.Value.Dequeue();
                    queueData.Enqueue(_messageMapper.InverseMap(message));
                }
            }
        }

        public Exchange GetExchangeFor(Message message)
        {
            var exchangeData =
                _exchangeDataStorage.Data.FirstOrDefault(excnage => excnage.Name.Equals(message.ExchangeName));
            return _exchangeMapper.Map(exchangeData);
        }

        public Message GetMessageFromQueueWithName(string queueName)
        {
            return _messageMapper.Map(_queuesData.Data[queueName].Dequeue());
        }
    }
}