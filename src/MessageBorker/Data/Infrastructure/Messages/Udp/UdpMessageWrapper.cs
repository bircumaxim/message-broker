using System.IO;
using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Messages.Udp
{
    public class UdpMessageWrapper : Message
    {
        public string ClientName { get; set; }
        public MemoryStream MemoryStream { get; set; }
        public byte[] Message{ get; set; }

        public UdpMessageWrapper()
        {
            MemoryStream = new MemoryStream();
        }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(ClientName);
            serializer.WriteByteArray(MemoryStream.ToArray());
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            ClientName = deserializer.ReadStringUtf8();
            Message = deserializer.ReadByteArray();
        }
    }
}