using System;

namespace Serialization
{
    public interface IDeserializer
    {
        byte ReadByte();
        byte[] ReadByteArray();
        int ReadInt32();
        uint ReadUInt32();
        long ReadInt64();
        bool ReadBoolean();
        DateTime ReadDateTime();
        char ReadCharUtf8();
        string ReadStringUtf8();
        T ReadObject<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : ISerializable;
        T[] ReadObjectArray<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : ISerializable;
    }
}