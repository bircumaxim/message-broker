using Persistence.Models;

namespace Persistence
{
    public interface IStorage
    {
        int StoreMessage(MessageRecord messageRecord);

        int RemoveMessage(int id);
    }
}