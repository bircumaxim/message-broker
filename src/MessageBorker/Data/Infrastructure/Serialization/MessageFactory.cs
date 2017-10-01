using System;
using System.Linq;
using System.Reflection;

namespace Serialization
{
    public class MessageFactory
    {
        private static MessageFactory _instance;
        public static MessageFactory Instance => _instance ?? (_instance = new MessageFactory());

        public Message CreateMessageByTypeId(int messageTypeId)
        {
            return Activator.CreateInstance(GetMessageType(messageTypeId)) as Message;
        }

        private static Type GetMessageType(int messageTypeId)
        {
            return Assembly.GetEntryAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.GetCustomAttribute<Serializable>().Id == messageTypeId);
        }
    }
}