namespace Serialization
{
    public interface ISerializable
    {
        void Serialize(ISerializer serializer);
        void Deserialize(IDeserializer deserializer);
    }
}