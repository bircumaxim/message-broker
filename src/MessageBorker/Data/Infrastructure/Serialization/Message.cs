using System;
using System.Collections.Generic;
using System.Reflection;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Serialization
{
    public abstract class Message : ISerializable
    {
        public string MessageId { get; set; }
        public string MessageTypeName => GetType().Name;
        
        protected Message()
        {
            MessageId = Guid.NewGuid().ToString();
        }

        public virtual void Serialize(ISerializer serializer)
        {
            serializer.WriteStringUtf8(MessageId);
        }

        public virtual void Deserialize(IDeserializer deserializer)
        {
            MessageId = deserializer.ReadStringUtf8();
        }
    }
}