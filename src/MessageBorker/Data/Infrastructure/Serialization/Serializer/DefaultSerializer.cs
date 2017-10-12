using System;
using System.IO;
using System.Text;

namespace Serialization.Serializer
{
    public class DefaultSerializer : Serializer
    {
        public DefaultSerializer(Stream stream) : base(stream)
        {
        }
        
        public override void WriteByteArray(byte[] bytes)
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
                    Write(bytes, 0, bytes.Length);
                }
            }
        }

        public override void WriteInt32(int number)
        {
            WriteByte((byte) ((number >> 24) & 0xFF));
            WriteByte((byte) ((number >> 16) & 0xFF));
            WriteByte((byte) ((number >> 8) & 0xFF));
            WriteByte((byte) (number & 0xFF));
        }

        public override void WriteUInt32(uint number)
        {
            WriteInt32((int) number);
        }

        public override void WriteInt64(long number)
        {
            WriteByte((byte) ((number >> 56) & 0xFF));
            WriteByte((byte) ((number >> 48) & 0xFF));
            WriteByte((byte) ((number >> 40) & 0xFF));
            WriteByte((byte) ((number >> 32) & 0xFF));
            WriteByte((byte) ((number >> 24) & 0xFF));
            WriteByte((byte) ((number >> 16) & 0xFF));
            WriteByte((byte) ((number >> 8) & 0xFF));
            WriteByte((byte) (number & 0xFF));
        }

        public override void WriteBoolean(bool b)
        {
            WriteByte((byte) (b ? 1 : 0));
        }

        public override void WriteDateTime(DateTime dateTime)
        {
            WriteInt64(dateTime.Ticks);
        }

        public override void WriteCharUtf8(char c)
        {
            WriteByteArray(Encoding.UTF8.GetBytes(c.ToString()));
        }

        public override void WriteStringUtf8(string text)
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

        public override void WriteObject(ISerializable serializableObject)
        {
            if (serializableObject == null)
            {
                WriteByte(0);
                return;
            }

            WriteByte(1);
            serializableObject.Serialize(this);
        }

        public override void WriteObjectArray(ISerializable[] serializableObjects)
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