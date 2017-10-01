using System;
using System.Reflection;

namespace Serialization
{
    public abstract class Message : ISerializable
    {
        public string MessageId { get; set; }
        public string RepliedMessageId { get; set; }
        public int MessageTypeId => GetType().GetCustomAttribute<Serializable>().Id;

        protected Message()
        {
            MessageId = Guid.NewGuid().ToString();
        }

        public virtual void Serialize(ISerializer serializer)
        {
            serializer.WriteStringUtf8(MessageId);
            serializer.WriteStringUtf8(RepliedMessageId);
        }

        public virtual void Deserialize(IDeserializer deserializer)
        {
            MessageId = deserializer.ReadStringUtf8();
            RepliedMessageId = deserializer.ReadStringUtf8();
        }
    }
}