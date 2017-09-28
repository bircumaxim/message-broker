namespace Serialization
{
    public interface IWireProtocol
    {
        void WriteMessage(ISerializer serializer, Message message);

        Message ReadMessage(IDeserializer deserializer);
    }
}