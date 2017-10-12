using System;
using System.Collections.Generic;

namespace Persistence.Storages.MemoryStorage
{
    public class MemoryStorageFactory
    {
        private static MemoryStorageFactory _instance;
        public static MemoryStorageFactory Instance => _instance ?? (_instance = new MemoryStorageFactory());

        private readonly Dictionary<Type, object> _storages;

        private MemoryStorageFactory()
        {
            _storages = new Dictionary<Type, object>();
        }

        public Storrage<T> GetStorrageFor<T>(Type type) where T : class
        {
            object storage;
            if (!_storages.TryGetValue(type, out storage))
            {
                storage = Activator.CreateInstance(type);
                _storages.Add(type, storage);
            }
            return new Storrage<T> {Data = storage as T};
        }
    }
}