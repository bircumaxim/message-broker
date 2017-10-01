using System;
using System.IO;
using System.Text;

namespace Serialization.Serializers
{
    public class DefaultSerializer : ISerializer
    {
        private readonly Stream _stream;

        public DefaultSerializer(Stream stream)
        {
            _stream = stream;
        }

        public void WriteByte(byte b)
        {
            _stream.WriteByte(b);
        }

        public void WriteByteArray(byte[] bytes)
        {
            if (bytes == null)
            {
                WriteInt32(-1);
            }
            else
            {
                WriteInt32(bytes.Length);
                if (bytes.Length > 0)
                {
                    _stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public void WriteInt32(int number)
        {
            _stream.WriteByte((byte) ((number >> 24) & 0xFF));
            _stream.WriteByte((byte) ((number >> 16) & 0xFF));
            _stream.WriteByte((byte) ((number >> 8) & 0xFF));
            _stream.WriteByte((byte) (number & 0xFF));
        }

        public void WriteUInt32(uint number)
        {
            WriteInt32((int) number);
        }

        public void WriteInt64(long number)
        {
            _stream.WriteByte((byte) ((number >> 56) & 0xFF));
            _stream.WriteByte((byte) ((number >> 48) & 0xFF));
            _stream.WriteByte((byte) ((number >> 40) & 0xFF));
            _stream.WriteByte((byte) ((number >> 32) & 0xFF));
            _stream.WriteByte((byte) ((number >> 24) & 0xFF));
            _stream.WriteByte((byte) ((number >> 16) & 0xFF));
            _stream.WriteByte((byte) ((number >> 8) & 0xFF));
            _stream.WriteByte((byte) (number & 0xFF));
        }

        public void WriteBoolean(bool b)
        {
            _stream.WriteByte((byte) (b ? 1 : 0));
        }

        public void WriteDateTime(DateTime dateTime)
        {
            WriteInt64(dateTime.Ticks);
        }

        public void WriteCharUtf8(char c)
        {
            WriteByteArray(Encoding.UTF8.GetBytes(c.ToString()));
        }

        public void WriteStringUtf8(string text)
        {
            switch (text)
            {
                case null:
                    WriteInt32(-1);
                    break;
                case "":
                    WriteInt32(0);
                    break;
                default:
                    WriteByteArray(Encoding.UTF8.GetBytes(text));
                    break;
            }
        }

        public void WriteObject(ISerializable serializableObject)
        {
            if (serializableObject == null)
            {
                _stream.WriteByte(0);
                return;
            }

            _stream.WriteByte(1);
            serializableObject.Serialize(this);
        }

        public void WriteObjectArray(ISerializable[] serializableObjects)
        {
            if (serializableObjects == null)
            {
                WriteInt32(-1);
            }
            else
            {
                WriteInt32(serializableObjects.Length);
                foreach (var objectToSerialize in serializableObjects)
                {
                    WriteObject(objectToSerialize);
                }
            }
        }
    }
}