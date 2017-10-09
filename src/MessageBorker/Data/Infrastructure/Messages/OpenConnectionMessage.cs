using Serialization;

namespace Messages
{
    public class OpenConnectionMessage : Message
    {
        public string ClientName { get; set; }
        public string ClientIp { get; set; }
        public int ClientPort{ get; set; }
        
        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(ClientName);
            serializer.WriteStringUtf8(ClientIp);
            serializer.WriteInt32(ClientPort);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            ClientName = deserializer.ReadStringUtf8();
            ClientName = deserializer.ReadStringUtf8();
            ClientPort = deserializer.ReadInt32();
        }
    }
}