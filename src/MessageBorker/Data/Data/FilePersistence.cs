using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Configuration;
using Data.Mappers;
using Data.Models;
using log4net;
using Messages.Payload;
using Persistence.Storages.FileStorage;
using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;
using Serialization.WireProtocol;

namespace Data
{
    public class FilePersistence
    {
        private readonly ILog _logger;
        private readonly FileStorage _fileStorage;
        private readonly IWireProtocol _wireProtocol;
        private readonly string _rootDirectory;
        private readonly PayloadMessageMapper _payloadMessageMapper;

        public FilePersistence(IConfiguration configuration)
        {
            _payloadMessageMapper = new PayloadMessageMapper();
            _fileStorage = new FileStorage();
            _wireProtocol = configuration.GetPersistenceWireProtocol();
            _rootDirectory = configuration.GetFilePersistenceRootDirectory();
            _logger = LogManager.GetLogger(GetType());
        }

        public void PersistMessage(string queueKey, MessageData message)
        {
            var payloadMessage = _payloadMessageMapper.InverseMap(message);
            payloadMessage.TimeStamp = DateTime.Now;
            _fileStorage.SaveMessage(GetBytesFromMessage(payloadMessage), GetFileNameForMessage(payloadMessage));
            _logger.Debug($"Saved {message.GetType().Name} to Queue with id=\"{queueKey}\"");
        }

        public void DeleteMessage(string messageName)
        {
            _fileStorage.DeleteMessage(Path.Combine(_rootDirectory, messageName));
        }

        public List<PayloadMessage> GetAllMessages()
        {
            return _fileStorage.GetAllMessages(_rootDirectory)
                .Select(stream => _wireProtocol.ReadMessage(new DefaultDeserializer(stream)) as PayloadMessage)
                .OrderBy(message => message?.TimeStamp)
//                .Select(message => _payloadMessageMapper.Map(message))
                .ToList();
        }

        private byte[] GetBytesFromMessage(PayloadMessage payloadMessage)
        {
            var stream = new MemoryStream();
            _wireProtocol.WriteMessage(new DefaultSerializer(stream), payloadMessage);
            return stream.ToArray();
        }

        private string GetFileNameForMessage(PayloadMessage messageData)
        {
            return Path.Combine(_rootDirectory, messageData.MessageId);
        }
    }
}