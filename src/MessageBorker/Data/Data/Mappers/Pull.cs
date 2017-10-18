using System;
using System.Collections.Generic;

namespace Data.Mappers
{
    public class Pull<T>
    {
        private readonly Dictionary<Type, T> _objectHolder;
        private readonly Dictionary<Type, Type> _registeredTypes;

        public Pull(Dictionary<Type, Type> types)
        {
            _objectHolder = new Dictionary<Type, T>();
            _registeredTypes = types;
        }

        public bool TryGetObject(Type type, ref T obj)
        {
            var typeIsregistered = _registeredTypes.ContainsKey(type);
            if (typeIsregistered)
            {
                obj = GetObject(type);
            }
            return typeIsregistered;
        }

        public T GetObject(Type type)
        {
            if (!_objectHolder.ContainsKey(type))
            {
                _objectHolder.Add(type, (T) Activator.CreateInstance(GetObjectTypeFor(type)));
            }
            return _objectHolder[type];
        }

        public Type GetObjectTypeFor(Type type)
        {
            if (!_registeredTypes.ContainsKey(type))
            {
                throw new Exception($"There was not registered any mapper for such object {type}");
            }
            return _registeredTypes[type];
        }
    }
}