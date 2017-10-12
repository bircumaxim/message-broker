using System.Collections.Generic;
using Data.Configuration;
using Data.Models;
using Data.Models.Mappers;
using Domain.Exhcanges;
using Domain.GateWays;
using Messages.Payload;
using Message = Domain.Messages.Message;

namespace Data
{
    public class Persistence : IPersistenceGateWay
    {
        private readonly MessageMapper _messageMapper;
        private readonly ExchangeMapper _exchangeMapper;
        private readonly MemoryPersistence _memoryPersistence;
        private readonly FilePersistence _filePersistence;

        public Persistence(IConfiguration configuration)
        {
            _messageMapper = new MessageMapper();
            _exchangeMapper = new ExchangeMapper();
            _memoryPersistence = new MemoryPersistence(configuration);
            _filePersistence = new FilePersistence(configuration);
        }

        public void DeleteMessageWithName(string name)
        {
            _filePersistence.DeleteMessage(name);
        }
        
        public List<PayloadMessage> GetPerisistedMessages()
        {
            return _filePersistence.GetAllMessages();
        }

        public void PersistQueues(Dictionary<string, Domain.Queue<Message>> queues)
        {
            foreach (var queue in queues)
            {
                while (!queue.Value.IsEmpty())
                {
                    var message = queue.Value.Dequeue();
                    if (message.IsDurable)
                    {
                        _filePersistence.PersistMessage(queue.Key, _messageMapper.InverseMap(message));
                    }
                    _memoryPersistence.PersistMessage(queue.Key, _messageMapper.InverseMap(message));
                }
            }
        }

        public Exchange GetExchangeFor(Message message)
        {
            var exchange = _memoryPersistence.GetExchangeByName(message.ExchangeName);
            return _exchangeMapper.Map(exchange);
        }

        public Message GetMessageFromQueueWithName(string queueName)
        {
            var message = _memoryPersistence.GetMessageFromQueueWithName(queueName);
            return _messageMapper.Map(message);
        }

        public ServerGeneralInfo GetServerInfo()
        {
            return _memoryPersistence.GetServerGeneralInfo();
        }
    }
}