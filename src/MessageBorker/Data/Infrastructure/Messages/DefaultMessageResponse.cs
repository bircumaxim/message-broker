using Serialization;

namespace Messages
{
    public class DefaultMessageResponse : Message
    {
        public byte[] Payload { get; set; }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteByteArray(Payload);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            Payload = deserializer.ReadByteArray();
        }
    }
}