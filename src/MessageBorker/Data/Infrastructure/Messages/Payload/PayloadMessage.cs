using System;
using System.IO;
using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Messages.Payload
{
    public class PayloadMessage : Message
    {
        public string DestinationQueueName { get; set; }
        public DateTime TimeStamp { get; set; }
        public byte[] Payload { get; set; }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(DestinationQueueName);
            serializer.WriteDateTime(TimeStamp);
            serializer.WriteByteArray(Payload);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            DestinationQueueName = deserializer.ReadStringUtf8();
            TimeStamp = deserializer.ReadDateTime();
            Payload = deserializer.ReadByteArray();
        }
    }
}