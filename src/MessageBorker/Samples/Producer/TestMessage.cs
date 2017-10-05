using Serialization;

namespace Producer
{
    public class TestMessage : Message
    {
        public string Name { get; set; }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(Name);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            Name = deserializer.ReadStringUtf8();
        }
    }
}