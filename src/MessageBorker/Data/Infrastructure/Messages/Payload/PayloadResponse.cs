using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Messages.Payload
{
    public class DefaultMessageResponse : Message
    {
        public string ReceivedMessageId { get; set; }
        public byte[] Payload { get; set; }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(ReceivedMessageId);
            serializer.WriteByteArray(Payload);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            ReceivedMessageId = deserializer.ReadStringUtf8();
            Payload = deserializer.ReadByteArray();
        }
    }
}