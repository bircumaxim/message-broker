using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Messages.Connection
{
    public class OpenConnectionResponse : Message
    {
        public bool IsConnectionAccepted { get; set; }
        public string ClientName { get; set; }
        
        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteBoolean(IsConnectionAccepted);
            serializer.WriteStringUtf8(ClientName);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            IsConnectionAccepted = deserializer.ReadBoolean();
            ClientName = deserializer.ReadStringUtf8();
        }
    }
}