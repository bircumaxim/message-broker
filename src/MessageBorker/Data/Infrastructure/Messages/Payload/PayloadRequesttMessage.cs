﻿using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Messages.Payload
{
    public class PayloadRequestMessage : Message
    {
        public string QueueName { get; set; }
        
        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(QueueName);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            QueueName = deserializer.ReadStringUtf8();
        }
    }
}