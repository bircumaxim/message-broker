using System;
using log4net;
using Persistence.Storages;

namespace Persistence
{
    public class StorageFactory
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(StorageFactory));
        
        public static IStorage CreateStorageManager()
        {
            //Get a reference to the settings
            //TODO get a reference to a object with settings.

            //Create storage manager according to the settings
            var storageType = ""; //TODO read here storage type from settings.
            IStorage storage;
            if (storageType.Equals("FILE", StringComparison.OrdinalIgnoreCase))
            {
                storage = new FileStorage();
            }
            else //Default storage manager
            {
                storage =  new MemoryStorage();
            }
            return storage;
        }
    }
}