using System.IO;

namespace Transport.Serialization.Serializers
{
    public class DefaultSerializer : ISerializer
    {
        private readonly MemoryStream _memoryStream;

        public DefaultSerializer(MemoryStream memoryStream)
        {
            _memoryStream = memoryStream;
        }
        
        //TODO implmented default serializer
    }
}