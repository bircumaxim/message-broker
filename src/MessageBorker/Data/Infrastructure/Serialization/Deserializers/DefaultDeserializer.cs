using System;
using System.IO;
using System.Text;
using log4net;
using log4net.Core;

namespace Serialization.Deserializers
{
    public class DefaultDeserializer : IDeserializer
    {
        private readonly ILog _logger;
        private readonly Stream _stream;

        public DefaultDeserializer(Stream stream)
        {   
            _logger =  LogManager.GetLogger(GetType());
            _stream = stream;
        }

        public byte ReadByte()
        {   
            var b = _stream.ReadByte();
            if (b == -1)
            {
                _logger.Error("Can not read from stream! Input stream is closed.");
                throw new Exception("Can not read from stream! Input stream is closed.");
            }

            return (byte) b;
        }

        public byte[] ReadByteArray()
        {
            var length = ReadInt32();
            if (length < 0)
            {
                return null;
            }

            return length == 0 ? new byte[0] : ReadByteArray(length);
        }

        public int ReadInt32()
        {
            return (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
        }

        public uint ReadUInt32()
        {
            return (uint) ReadInt32();
        }

        public long ReadInt64()
        {
            return ((long) ReadByte() << 56) |
                   ((long) ReadByte() << 48) |
                   ((long) ReadByte() << 40) |
                   ((long) ReadByte() << 32) |
                   ((long) ReadByte() << 24) |
                   ((long) ReadByte() << 16) |
                   ((long) ReadByte() << 8)  | 
                   ReadByte();
        }

        public bool ReadBoolean()
        {
            return ReadByte() == 1;
        }

        public DateTime ReadDateTime()
        {
            return new DateTime(ReadInt64());
        }

        public char ReadCharUtf8()
        {
            return ReadStringUtf8()[0];
        }

        public string ReadStringUtf8()
        {
            var length = ReadInt32();
            return length < 0 ? null : (length == 0 ? "" : Encoding.UTF8.GetString(ReadByteArray(length), 0, length));
        }

        public T ReadObject<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : ISerializable
        {
            if (ReadByte() == 0)
            {
                return default(T);
            }
            var serializableObject = createObjectHandler();
            serializableObject.Deserialize(this);
            return serializableObject;
        }

        public T[] ReadObjectArray<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : ISerializable
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
                var read = _stream.Read(buffer, totalRead, length - totalRead);
                if (read <= 0)
                {
                    _logger.Error("Can not read from stream! Input stream is closed.");
                    throw new Exception("Can not read from stream! Input stream is closed.");
                }

                totalRead += read;
            }

            return buffer;
        }
    }
}