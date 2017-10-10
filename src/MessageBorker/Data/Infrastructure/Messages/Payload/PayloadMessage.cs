using System.IO;
using Serialization;

namespace Messages.Payload
{
    public class PayloadMessage : Message
    {
        public bool IsDurable { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeName { get; set; }
        public MemoryStream MemoryStream { get; }
        public byte[] Payload { get; set; }

        public PayloadMessage()
        {
            MemoryStream = new MemoryStream();
        }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteBoolean(IsDurable);
            serializer.WriteStringUtf8(RoutingKey);
            serializer.WriteStringUtf8(ExchangeName);
            serializer.WriteByteArray(MemoryStream.ToArray());
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            IsDurable = deserializer.ReadBoolean();
            RoutingKey = deserializer.ReadStringUtf8();
            ExchangeName = deserializer.ReadStringUtf8();
            Payload = deserializer.ReadByteArray();
        }
    }
}