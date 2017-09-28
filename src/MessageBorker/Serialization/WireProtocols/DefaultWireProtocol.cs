using System;
using System.Reflection;

namespace Serialization.WireProtocols
{
    public class DefaultWireProtocol : IWireProtocol
    {
        public const uint DefaultProtocolType = 19180685;
        
        public void WriteMessage(ISerializer serializer, Message message)
        {
            //Write protocol type
            serializer.WriteUInt32(DefaultProtocolType);
            
            //Write the message type
            serializer.WriteInt32(message.GetType().GetCustomAttribute<Serializable>().Id);
            
            //Write message
            serializer.WriteObject(message);
        }

        public Message ReadMessage(IDeserializer deserializer)
        {
            //Read protocol type
            var protocolType = deserializer.ReadUInt32();
            if (protocolType != DefaultProtocolType)
            {
                throw new Exception("Wrong protocol type: " + protocolType + ".");
            }

            //Read message type
            var messageTypeId = deserializer.ReadInt32();

            //Read and return message
            return deserializer.ReadObject(() => MessageFactory.Instance.CreateMessageByTypeId(messageTypeId));
        }
    }
}