#nullable enable
namespace UniT.Data.Storage
{
    using System;

    public abstract class DataStorage<TRawData> : IDataStorage
    {
        Type IDataStorage.RawDataType => typeof(TRawData);

        bool IDataStorage.CanStore(Type type) => this.CanStore(type);

        protected abstract bool CanStore(Type type);
    }
}