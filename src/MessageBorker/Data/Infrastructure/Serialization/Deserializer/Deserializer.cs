using System;
using System.IO;

namespace Serialization.Deserializer
{
    public abstract class Deserializer : IDeserializer
    {
        private readonly Stream _stream;

        protected Deserializer(Stream stream)
        {
            _stream = stream;
        }

        public void Decrypt(DecryptFunction decryptFunction, string secretKey)
        {
            if (decryptFunction == null)
            {
                throw new Exception("You didn't specified any decryption function !");
            }
            decryptFunction(_stream, secretKey);
        }

        public byte ReadByte()
        {
            var b = _stream.ReadByte();
            if (b == -1)
            {
                throw new Exception("Can not read from stream! Input stream is closed.");
            }

            return (byte) b;
        }
        public int Read(int length, byte[] buffer, int totalRead)
        {
            return _stream.Read(buffer, totalRead, length - totalRead);
        }
        
        public abstract byte[] ReadByteArray();
        public abstract int ReadInt32();
        public abstract uint ReadUInt32();
        public abstract long ReadInt64();
        public abstract bool ReadBoolean();
        public abstract DateTime ReadDateTime();
        public abstract char ReadCharUtf8();
        public abstract string ReadStringUtf8();
        public abstract T ReadObject<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : ISerializable;
        public abstract T[] ReadObjectArray<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : ISerializable;
    }
}