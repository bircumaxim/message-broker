using System;
using System.Collections.Generic;
using System.Linq;
using Data.Configuration;
using Data.Models;
using Data.Models.Mappers;
using Domain.Exhcanges;
using Domain.GateWays;
using Persistence.Storages;
using Message = Domain.Messages.Message;

namespace Data
{
    public class Persistence : IPersistenceGateWay
    {
        private readonly MessageMapper _messageMapper;
        private readonly ExchangeMapper _exchangeMapper;
        private readonly Storrage<List<ExchangeData>> _exchangesStorage;
        private readonly Storrage<Dictionary<string, QueueData<MessageData>>> _queuesStorrage;
        private readonly Storrage<ServerGeneralInfo> _serrverInfoStorrage;

        public Persistence(IConfiguration configuration)
        {
            _messageMapper = new MessageMapper();
            _exchangeMapper = new ExchangeMapper();

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


        public void PersistQueues(Dictionary<string, Domain.Queue<Message>> queues)
        {
            foreach (var queue in queues)
            {
                var queueData = _queuesStorrage.Data[queue.Key];
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
                _exchangesStorage.Data.FirstOrDefault(excnage => excnage.Name.Equals(message.ExchangeName));
            return _exchangeMapper.Map(exchangeData);
        }

        public Message GetMessageFromQueueWithName(string queueName)
        {
            return _messageMapper.Map(_queuesStorrage.Data[queueName].Dequeue());
        }

        public ServerGeneralInfo GetServerInfo()
        {
            _serrverInfoStorrage.Data.MessagesInQueue = _queuesStorrage.Data.Values
                .Where(queue => queue != null)
                .Sum(queue => queue.Count());
            return _serrverInfoStorrage.Data;
        }
    }
}