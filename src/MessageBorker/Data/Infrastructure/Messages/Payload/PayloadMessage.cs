using System;
using System.IO;
using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Messages.Payload
{
    public class PayloadMessage : Message
    {
        public DateTime TimeStamp { get; set; }
        public bool IsDurable { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeName { get; set; }
        public MemoryStream MemoryStream { get; set; }
        public byte[] Payload { get; set; }

        public PayloadMessage()
        {
            MemoryStream = new MemoryStream();
        }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteDateTime(TimeStamp);
            serializer.WriteBoolean(IsDurable);
            serializer.WriteStringUtf8(RoutingKey);
            serializer.WriteStringUtf8(ExchangeName);
            serializer.WriteByteArray(MemoryStream.ToArray());
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            TimeStamp = deserializer.ReadDateTime();
            IsDurable = deserializer.ReadBoolean();
            RoutingKey = deserializer.ReadStringUtf8();
            ExchangeName = deserializer.ReadStringUtf8();
            Payload = deserializer.ReadByteArray();
        }
    }
}