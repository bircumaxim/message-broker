using System;
using System.Collections.Generic;
using Data.Mappers;
using Data.Mappers.Persistence;
using Data.Models;
using Domain.Exhcanges;
using Domain.GateWays;
using Domain.Models;
using Messages.Payload;
using Persistence;
using Persistence.Configuration;

namespace Data
{
    public class Persistence : IPersistenceGateWay
    {
        private readonly PersistenceMessageMapper _persistenceMessageMapper;
        private readonly RouteMessageMapper _routeMessageMapper;
        private readonly PersitenceServerGeneralInfoMapper _persitenceServerGeneralInfoMapper;
        private readonly PersistenceExchangeMapper _persistenceExchangeMapper;
        private readonly MemoryPersistence _memoryPersistence;
        private readonly FilePersistence _filePersistence;

        public Persistence(IPersistenceConfiguration configuration)
        {
            _persistenceMessageMapper = new PersistenceMessageMapper();
            _routeMessageMapper = new RouteMessageMapper();
            _persistenceExchangeMapper = new PersistenceExchangeMapper();
            _memoryPersistence = new MemoryPersistence(configuration);
            _persitenceServerGeneralInfoMapper = new PersitenceServerGeneralInfoMapper();
            _filePersistence = new FilePersistence(configuration);

            GetPerisistedMessages();
        }

        public void DeleteMessageWithName(string name)
        {
            _filePersistence.DeleteMessage(name);
        }

        public void GetPerisistedMessages()
        {
            _filePersistence.GetAllMessages()
                .ForEach(message => _memoryPersistence.PersistMessage(message.DestinationQueueName, message));
        }

        public void PersistQueues(Dictionary<string, Domain.Queue<RouteMessage>> queues)
        {
            foreach (var queue in queues)
            {
                while (!queue.Value.IsEmpty())
                {
                    var message = queue.Value.Dequeue();
                    var persistenceMessage = _routeMessageMapper.Map(message);
                    if (message.IsDurable)
                    {
                        _filePersistence.PersistMessage(queue.Key, persistenceMessage);
                    }
                    _memoryPersistence.PersistMessage(queue.Key, persistenceMessage);
                }
            }
        }

        public Exchange GetExchangeFor(RouteMessage routeMessage)
        {
            var exchange = _memoryPersistence.GetExchangeByName(routeMessage.ExchangeName);
            return _persistenceExchangeMapper.Map(exchange);
        }

        public Message GetMessageFromQueueWithName(string queueName)
        {
            var persistenceMessage = _memoryPersistence.GetMessageFromQueueWithName(queueName);
            return _persistenceMessageMapper.Map(persistenceMessage);
        }

        public ServerGeneralInfo GetServerInfo()
        {
            return _persitenceServerGeneralInfoMapper.Map(_memoryPersistence.GetServerGeneralInfo());
        }
    }
}