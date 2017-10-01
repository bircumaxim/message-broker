namespace Serialization
{
    public delegate T CreateSerializableObjectHandler<out T>() where T : ISerializable;
}
