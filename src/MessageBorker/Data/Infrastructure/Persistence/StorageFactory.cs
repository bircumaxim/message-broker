using System;
using Persistence.Storages;

namespace Persistence
{
    public class StorageFactory
    {
        public static IStorage CreateStorageManager()
        {
            //Get a reference to the settings
            //TODO get a reference to a object with settings.

            //Create storage manager according to the settings
            var storageType = ""; //TODO read here storage type from settings.
            IStorage storage = null;
            if (storageType.Equals("FILE", StringComparison.OrdinalIgnoreCase))
            {
                storage = new FileStorage();
            }
            else //Default storage manager
            {
                storage =  new MemoryStorage();
            }

            if (storage == null)
            {
                //TODO log here exception.
                throw  new NullReferenceException();
            }
            return storage;
        }
    }
}