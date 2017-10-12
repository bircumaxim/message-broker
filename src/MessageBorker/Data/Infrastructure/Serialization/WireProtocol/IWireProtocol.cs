using Serialization.Deserializer;
using Serialization.Serializer;

namespace Serialization.WireProtocol
{
    public interface IWireProtocol
    {
        void WriteMessage(ISerializer serializer, Message message);

        Message ReadMessage(IDeserializer deserializer);
    }
}