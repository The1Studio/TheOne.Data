#nullable enable
namespace UniT.Data.Storage
{
    using System;

    public interface IDataStorage
    {
        public Type RawDataType { get; }

        public bool CanStore(Type type);
    }
}