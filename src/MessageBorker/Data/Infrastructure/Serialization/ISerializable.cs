using Serialization.Deserializer;
using Serialization.Serializer;

namespace Serialization
{
    public interface ISerializable
    {
        void Serialize(ISerializer serializer);
        void Deserialize(IDeserializer deserializer);
    }
}