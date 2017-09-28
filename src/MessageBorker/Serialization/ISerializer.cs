using System;

namespace Serialization
{
    public interface ISerializer
    {
        void WriteByte(byte b);
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