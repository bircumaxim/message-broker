namespace Transport.Serialization.WireProtocols
{
    public class DefaultWireProtocol : IWireProtocol
    {
        public void WriteMessage(ISerializer serializer, Message message)
        {
            throw new System.NotImplementedException();
        }

        public Message ReadMessage(IDeserializer deserializer)
        {
            throw new System.NotImplementedException();
        }
        
        //TODO implmented default wire protocol
    }
}