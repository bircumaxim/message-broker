using System.IO;

namespace Transport.Serialization.Deserializers
{
    public class DefaultDeserializer : IDeserializer
    {
        private readonly Stream _stream;

        public DefaultDeserializer(Stream stream)
        {
            _stream = stream;
        }
        
        //TODO implmented default deserializer
    }
}