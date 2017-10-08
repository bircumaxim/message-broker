using System;
using System.Linq;
using System.Reflection;
using log4net;

namespace Serialization
{
    public class MessageFactory
    {
        private readonly ILog _logger;
        private static MessageFactory _instance;
        public static MessageFactory Instance => _instance ?? (_instance = new MessageFactory());

        private MessageFactory()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        public Message CreateMessageByName(string messageTypeName)
        {
            Message message;
            Type messageType;
            if (TryGetMessageType(messageTypeName, out messageType))
            {
                message = Activator.CreateInstance(messageType) as Message;
            }
            else
            {
                throw new NullReferenceException($"Unknown message type {messageTypeName}");
            }
            return message;
        }

        private bool TryGetMessageType(string messageTypeName, out Type messageType)
        {
            Assembly loaddedModule = null;
            try
            {
                loaddedModule = AppDomain.CurrentDomain.Load(Configuration.Instance.ObjectsToSerializeModule);
            }
            catch (Exception e)
            {
                _logger.Error("There is no susch module");
            }
            messageType = loaddedModule?.GetTypes()
                              .FirstOrDefault(type => type.Name.Equals(messageTypeName)) ?? Assembly.GetEntryAssembly()
                              .GetTypes()
                              .FirstOrDefault(type => type.Name.Equals(messageTypeName)) ?? AppDomain.CurrentDomain
                              .GetAssemblies()
                              .SelectMany(assembly => assembly.GetTypes())
                              .FirstOrDefault(type => type.Name == messageTypeName);
            return messageType != null;
        }
    }
}