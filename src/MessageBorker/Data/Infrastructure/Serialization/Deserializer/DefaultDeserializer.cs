using System;
using System.IO;
using System.Text;

namespace Serialization.Deserializer
{
    public class DefaultDeserializer : Deserializer
    {
        public DefaultDeserializer(Stream stream) : base(stream)
        {
        }

        public override byte[] ReadByteArray()
        {
            var length = ReadInt32();
            if (length < 0)
            {
                return null;
            }
            return length == 0 ? new byte[0] : ReadByteArray(length);
        }
        
        public override int ReadInt32()
        {
            return (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
        }

        public override uint ReadUInt32()
        {
            return (uint) ReadInt32();
        }

        public override long ReadInt64()
        {
            return ((long) ReadByte() << 56) |
                   ((long) ReadByte() << 48) |
                   ((long) ReadByte() << 40) |
                   ((long) ReadByte() << 32) |
                   ((long) ReadByte() << 24) |
                   ((long) ReadByte() << 16) |
                   ((long) ReadByte() << 8) |
                   ReadByte();
        }

        public override bool ReadBoolean()
        {
            return ReadByte() == 1;
        }

        public override DateTime ReadDateTime()
        {
            return new DateTime(ReadInt64());
        }

        public override char ReadCharUtf8()
        {
            return ReadStringUtf8()[0];
        }

        public override string ReadStringUtf8()
        {
            var length = ReadInt32();
            return length < 0 ? null : (length == 0 ? "" : Encoding.UTF8.GetString(ReadByteArray(length), 0, length));
        }

        public override T ReadObject<T>(CreateSerializableObjectHandler<T> createObjectHandler)
        {
            if (ReadByte() == 0)
            {
                return default(T);
            }
            var serializableObject = createObjectHandler();
            serializableObject.Deserialize(this);
            return serializableObject;
        }

        public override T[] ReadObjectArray<T>(CreateSerializableObjectHandler<T> createObjectHandler)
        {
            var length = ReadInt32();
            if (length < 0)
            {
                return null;
            }

            if (length == 0)
            {
                return new T[0];
            }

            var objects = new T[length];
            for (var i = 0; i < length; i++)
            {
                objects[i] = ReadObject(createObjectHandler);
            }

            return objects;
        }

        private byte[] ReadByteArray(int length)
        {
            var buffer = new byte[length];
            var totalRead = 0;
            while (totalRead < length)
            {
                var read = Read(length, buffer, totalRead);
                if (read <= 0)
                {
                    throw new Exception("Can not read from stream! Input stream is closed.");
                }

                totalRead += read;
            }

            return buffer;
        }
    }
}