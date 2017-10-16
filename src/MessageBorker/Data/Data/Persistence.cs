using System.Collections.Generic;
using Data.Events;
using Data.Mappers;
using Data.Models;
using Domain.Exhcanges;
using Domain.GateWays;
using Domain.Models;
using Messages.Subscribe;
using Persistence;
using Persistence.Configuration;
using Persistence.Models;

namespace Data
{
    public class Persistence : IPersistenceGateWay
    {
        private readonly MemoryPersistence _memoryPersistence;
        private readonly FilePersistence _filePersistence;
        public event QueueUpdatedEventHandler QueueUpdate;
        
        public Persistence(IPersistenceConfiguration configuration)
        {
            _memoryPersistence = new MemoryPersistence(configuration);
            _filePersistence = new FilePersistence(configuration);

            GetPerisistedMessages();
        }

        public void DeleteMessageWithId(string messageId)
        {
            _filePersistence.DeleteMessage(messageId);
        }

        public void GetPersitedSubscriptions()
        {
            //TODO add subscriptions to memory and send them messages if they are alive.
        }
        
        public void GetPerisistedMessages()
        {
            _filePersistence.GetAllMessages()
                .ForEach(message => _memoryPersistence.PersistMessage(message.DestinationQueueName, message));
        }

        public void PersistSubscription(SubscribeMessage subscribeMessage, string subscriberName)
        {
            var subscription = MappersPull.Instance.Map<SubscribeMessage, PersistenceSubscription>(subscribeMessage);
            subscription.SubscriberName = subscriberName;
            if (subscribeMessage.IsDurable)
            {
                _filePersistence.PersistSubscription(subscription);
            }
            _memoryPersistence.PersistSubscription(subscription);
        }

        public void PersistQueues(Dictionary<string, Domain.Queue<RouteMessage>> queues)
        {
            foreach (var queue in queues)
            {
                while (!queue.Value.IsEmpty())
                {
                    var message = queue.Value.Dequeue();
                    var persistenceMessage = MappersPull.Instance.Map<RouteMessage, PersistenceMessage>(message);
                    if (message.IsDurable)
                    {
                        _filePersistence.PersistMessage(queue.Key, persistenceMessage);
                    }
                    _memoryPersistence.PersistMessage(queue.Key, persistenceMessage);
                    QueueUpdate?.Invoke(this, new QueueUpdateEventArgs(queue.Key));
                }
            }
        }

        public Exchange GetExchangeFor(RouteMessage routeMessage)
        {
            var exchange = _memoryPersistence.GetExchangeByName(routeMessage.ExchangeName);
            return MappersPull.Instance.Map<PersistenceExchange, Exchange>(exchange);
        }

        public Message GetMessageFromQueueWithName(string queueName)
        {
            var persistenceMessage = _memoryPersistence.GetMessageFromQueueWithName(queueName);
            return MappersPull.Instance.Map<PersistenceMessage, Message>(persistenceMessage);
        }

        public ServerGeneralInfo GetServerInfo()
        {
            return MappersPull.Instance.Map<PersistenceServerGeneralInfo, ServerGeneralInfo>(_memoryPersistence
                .GetServerGeneralInfo());
        }
    }
}