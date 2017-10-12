using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Messages.Udp
{
    public class UdpInitMessageRequest : Message
    {
        public string ClientIp { get; set; }
        public int ClientPort{ get; set; }
        
        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(ClientIp);
            serializer.WriteInt32(ClientPort);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            ClientIp = deserializer.ReadStringUtf8();
            ClientPort = deserializer.ReadInt32();
        }
    }
}