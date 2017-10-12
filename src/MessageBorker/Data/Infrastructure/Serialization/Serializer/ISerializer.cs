using System;
using System.IO;

namespace Serialization.Serializer
{
    public delegate void EncryptFunction(Stream clearData, string secretKey);
    
    public interface ISerializer
    {
        void Encrypt(EncryptFunction encryptFunction, string secretKey);
        void WriteByte(byte b);
        void Write(byte[] bytes, int offset, int count);
        void WriteByteArray(byte[] bytes);
        void WriteInt32(int number);
        void WriteUInt32(uint number);
        void WriteInt64(long number);
        void WriteBoolean(bool b);
        void WriteDateTime(DateTime dateTime);
        void WriteCharUtf8(char c);
        void WriteStringUtf8(string text);
        void WriteObject(ISerializable serializableObject);
        void WriteObjectArray(ISerializable[] serializableObjects);
    }
}