using System;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Serialization.WireProtocol
{
    public class DefaultWireProtocol : IWireProtocol
    {
        public const uint DefaultProtocolType = 19180685;
        private readonly bool _isCryptingEnabled;


        public DefaultWireProtocol(bool isCryptingEnabled = false)
        {
            _isCryptingEnabled = isCryptingEnabled;
        }

        public void WriteMessage(ISerializer serializer, Message message)
        {
            //Write protocol type
            serializer.WriteUInt32(DefaultProtocolType);

            //Write the message type
            serializer.WriteStringUtf8(message.GetType().Name);

            //Write message
            serializer.WriteObject(message);

            if (_isCryptingEnabled)
            {
                serializer.Encrypt(EncDec.Encrypt, "SECRET_KEY");
            }
        }

        public Message ReadMessage(IDeserializer deserializer)
        {
            //Read protocol type
            if (_isCryptingEnabled)
            {
                deserializer.Decrypt(EncDec.Decrypt, "SECRET_KEY");
            }
            
            var protocolType = deserializer.ReadUInt32();
            if (protocolType != DefaultProtocolType)
            {
                throw new Exception("Wrong protocol type: " + protocolType + ".");
            }

            //Read message type
            var messageTypeName = deserializer.ReadStringUtf8();

            //Read and return message
            return deserializer.ReadObject(() => MessageFactory.Instance.CreateMessageByName(messageTypeName));
        }
    }
}