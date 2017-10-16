using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Messages.Payload;
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

        public FilePersistence(IPersistenceConfiguration configuration)
        {
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

        public void DeleteMessage(string messageName)
        {
            _fileStorage.DeleteMessage(Path.Combine(_rootDirectory, messageName));
        }

        public List<PersistenceMessage> GetAllMessages()
        {
            var test = _fileStorage.GetAllMessages(_rootDirectory)
                .Select(stream => _wireProtocol.ReadMessage(new DefaultDeserializer(stream)) as PayloadMessage)
                .OrderBy(message => message?.TimeStamp)
                .Select(message => _payloadMessageMapper.InversMap(message))
                .ToList();
            return test;
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