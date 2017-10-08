using Serialization;

namespace Messages
{
    public class OpenConnectionMessage : Message
    {
        public string ClientName { get; set; }
        
        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(ClientName);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            ClientName = deserializer.ReadStringUtf8();
        }
    }
}