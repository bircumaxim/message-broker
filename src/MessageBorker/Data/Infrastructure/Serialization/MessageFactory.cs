using System;
using System.Linq;
using System.Reflection;

namespace Serialization
{
    public class MessageFactory
    {
        private static MessageFactory _instance;
        public static MessageFactory Instance => _instance ?? (_instance = new MessageFactory());

        private MessageFactory()
        {
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
            //TODO extract defautl assembly to settings !!!
            messageType = AppDomain.CurrentDomain.Load("Messages")
                              .GetTypes()
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