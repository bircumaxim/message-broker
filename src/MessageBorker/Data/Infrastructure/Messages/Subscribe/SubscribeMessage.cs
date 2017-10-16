using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Messages.Subscribe
{
    public class SubscribeMessage : Message
    {
        public bool IsDurable { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
        
        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteBoolean(IsDurable);
            serializer.WriteStringUtf8(Ip);
            serializer.WriteInt32(Port);
            serializer.WriteStringUtf8(QueueName);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            IsDurable = deserializer.ReadBoolean();
            Ip = deserializer.ReadStringUtf8();
            Port = deserializer.ReadInt32();
            QueueName = deserializer.ReadStringUtf8();
        }
    }
}