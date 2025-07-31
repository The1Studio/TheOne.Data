#nullable enable
namespace UniT.Data.Storage
{
    using System;

    public abstract class RuntimeWritableDataStorage<TRawData> : WritableDataStorage<TRawData>, IRuntimeWritableDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IRuntimeWritableData).IsAssignableFrom(type);
    }
}