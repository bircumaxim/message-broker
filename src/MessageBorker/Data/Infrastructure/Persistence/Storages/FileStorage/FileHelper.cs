using System.IO;

namespace Persistence.Storages.FileStorage
{
    public class FileHelper
    {
        public static void WriteBytesToFile(byte[] message, string fileName)
        {
            if (!File.Exists(fileName))
            {
                var directoryName = Path.GetDirectoryName(fileName);
                if (directoryName != null)
                {
                    Directory.CreateDirectory(directoryName);
                }
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    writer.Write(message.Length);
                    writer.Write(message);
                }
            }
        }

        public static byte[] ReadBytesFromFile(string fileName)
        {
            byte[] message = null;

            if (File.Exists(fileName))
            {
                using (var reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    int bytesCount = reader.ReadInt32();
                    message = reader.ReadBytes(bytesCount);
                }
            }

            return message;
        }

        public static void DeleteFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}