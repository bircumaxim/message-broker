using System;
using System.IO;

namespace Serialization.Serializer
{
    public abstract class Serializer : ISerializer
    {
        private readonly Stream _stream;
        
        protected Serializer(Stream stream)
        {
            _stream = stream;
        }

        public void Encrypt(EncryptFunction encryptFunction, string secretKey)
        {
            if (encryptFunction == null)
            {
                throw new Exception("You didn't specified any ecript function !");
            }
            encryptFunction(_stream, secretKey);
        }

        public void WriteByte(byte b)
        {
            _stream.WriteByte(b);
        }

        public void Write(byte[] bytes, int offset, int count)
        {
            _stream.Write(bytes, 0, bytes.Length);
        }

        public abstract void WriteByteArray(byte[] bytes);
        public abstract void WriteInt32(int number);
        public abstract void WriteUInt32(uint number);
        public abstract void WriteInt64(long number);
        public abstract void WriteBoolean(bool b);
        public abstract void WriteDateTime(DateTime dateTime);
        public abstract void WriteCharUtf8(char c);
        public abstract void WriteStringUtf8(string text);
        public abstract void WriteObject(ISerializable serializableObject);
        public abstract void WriteObjectArray(ISerializable[] serializableObjects);
    }
}