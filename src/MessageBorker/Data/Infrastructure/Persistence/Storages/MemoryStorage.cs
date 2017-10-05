using Persistence.Models;

namespace Persistence.Storages
{
    public class MemoryStorage : IStorage
    {   
        public int StoreMessage(MessageRecord messageRecord)
        {
            throw new System.NotImplementedException();
        }

        public int RemoveMessage(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}