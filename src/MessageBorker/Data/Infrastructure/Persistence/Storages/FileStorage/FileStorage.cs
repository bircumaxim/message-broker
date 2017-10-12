using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Persistence.Storages.FileStorage
{
    public class FileStorage
    {
        public void SaveMessage(byte[] message, string fileName)
        {
            FileHelper.WriteBytesToFile(message, fileName);
        }

        public MemoryStream GetMessageByName(string messageName)
        {
            return new MemoryStream(FileHelper.ReadBytesFromFile(messageName));
        }
        
        public void DeleteMessage(string fileName)
        {
            FileHelper.DeleteFile(fileName);
        }

        public List<MemoryStream> GetAllMessages(string directoryName)
        {
            var messages = new List<MemoryStream>();
            if (Directory.Exists(directoryName))
            {
                var fileNames = Directory.GetFiles(directoryName);
                messages.AddRange(fileNames.Select(GetMessageByName));
            }
            return messages;
        }
    }
}