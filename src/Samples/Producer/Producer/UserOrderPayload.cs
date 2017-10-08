using Serialization;

namespace Producer
{
    public class UserOrderPayload : Message
    {
        public string UserName { get; set; }
        public int OrderId { get; set; }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(UserName);
            serializer.WriteInt32(OrderId);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            UserName = deserializer.ReadStringUtf8();
            OrderId = deserializer.ReadInt32();
        }
    }
}