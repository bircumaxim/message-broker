using System;

namespace Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Serializable : Attribute
    {
        public int Id { get; set; }
    }
}