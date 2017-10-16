using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Messages.Payload;
using Messages.Subscribe;
using Persistence.Configuration;
using Persistence.Models;
using Persistence.Models.Mappers;
using Persistence.Storages.FileStorage;
using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;
using Serialization.WireProtocol;

namespace Persistence
{
    public class FilePersistence
    {
        private readonly ILog _logger;
        private readonly FileStorage _fileStorage;
        private readonly IWireProtocol _wireProtocol;
        private readonly string _rootDirectory;
        private readonly PayloadMessageMapper _payloadMessageMapper;
        private readonly SubscriptionMessageMapper _subscriptionMessageMapper;

        public FilePersistence(IPersistenceConfiguration configuration)
        {
            _subscriptionMessageMapper = new SubscriptionMessageMapper();
            _payloadMessageMapper = new PayloadMessageMapper();
            _fileStorage = new FileStorage();
            _wireProtocol = configuration.GetPersistenceWireProtocol();
            _rootDirectory = configuration.GetFilePersistenceRootDirectory();
            _logger = LogManager.GetLogger(GetType());
        }

        public void PersistMessage(string queueKey, PersistenceMessage message)
        {
            message.DestinationQueueName = queueKey;
            var payloadMessage = _payloadMessageMapper.Map(message);
            _fileStorage.SaveMessage(GetBytesFromMessage(payloadMessage), GetFileNameForMessage(message));
            _logger.Debug(
                $"Saved {message.GetType().Name} with id=\"{message.MessageId}\" to Queue with id=\"{queueKey}\"");
        }

        public void PersistSubscription(PersistenceSubscription subscription)
        {
            var subscribeMessage = _subscriptionMessageMapper.Map(subscription);
            _fileStorage.SaveMessage(GetBytesFromMessage(subscribeMessage),
                Path.Combine(_rootDirectory, subscription.SubscriberName));
            _logger.Debug($"Saved {subscription.GetType().Name} with id=\"{subscription.SubscriberName}\"");
        }

        public void DeleteMessage(string messageName)
        {
            _fileStorage.DeleteMessage(Path.Combine(_rootDirectory, messageName));
        }

        public void DeleteSubscribtion(string subscriberName)
        {
            //TODO implement here subscriber removing
        }

        public List<PersistenceMessage> GetAllMessages()
        {
            return _fileStorage.GetAllMessages(_rootDirectory)
                .Select(stream => _wireProtocol.ReadMessage(new DefaultDeserializer(stream)) as PayloadMessage)
                .OrderBy(message => message?.TimeStamp)
                .Select(message => _payloadMessageMapper.InversMap(message))
                .Where(message => message != null)
                .ToList();
        }

        public List<PersistenceSubscription> GetAllSubscriptions()
        {
            return _fileStorage.GetAllMessages(_rootDirectory)
                .Select(stream => _wireProtocol.ReadMessage(new DefaultDeserializer(stream)) as SubscribeMessage)
                .Select(message => _subscriptionMessageMapper.InversMap(message))
                .ToList();
        }

        private byte[] GetBytesFromMessage(Message message)
        {
            var stream = new MemoryStream();
            _wireProtocol.WriteMessage(new DefaultSerializer(stream), message);
            return stream.ToArray();
        }

        private string GetFileNameForMessage(PersistenceMessage messageData)
        {
            return Path.Combine(_rootDirectory, messageData.MessageId);
        }
    }
}