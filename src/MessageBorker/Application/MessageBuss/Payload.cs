using Serialization;

namespace MessageBuss
{
    public abstract class Payload : Message
    {
        public abstract override void Serialize(ISerializer serializer);

        public abstract override void Deserialize(IDeserializer deserializer);
    }
}