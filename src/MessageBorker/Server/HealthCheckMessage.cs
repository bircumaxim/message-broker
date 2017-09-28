using Serialization;

namespace Server
{
    [Serializable(Id = 1)]
    public class HealthCheckMessage : Message
    {
        public string SourceServerName { get; set; }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteStringUtf8(SourceServerName);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            SourceServerName = deserializer.ReadStringUtf8();
        }

        public override string ToString()
        {
            return SourceServerName;
        }   
    }
}