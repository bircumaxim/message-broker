using System;
using System.Collections.Generic;

namespace Data.Mappers
{
    public class ObjectPullBuilder<T>
    {
        private  Type _lasType;
        private readonly Dictionary<Type, Type> _types;

        public ObjectPullBuilder()
        {
            _types = new Dictionary<Type, Type>();
        }

        public ObjectPullBuilder<T> For(Type type)
        {
            _lasType = type;
            return this;
        }
        
        public ObjectPullBuilder<T> Use(Type t)
        {
            _types.Add(_lasType, t);
            return this;
        }

        public Pull<T> Build()
        {
            return new Pull<T>(_types);
        }
    }
}