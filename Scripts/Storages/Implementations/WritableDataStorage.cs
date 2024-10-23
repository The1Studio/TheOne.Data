#nullable enable
namespace TheOne.Data.Storage
{
    using System;

    public abstract class WritableDataStorage<TRawData> : EditorWritableDataStorage<TRawData>
    {
        protected override bool CanStore(Type type) => typeof(IWritableData).IsAssignableFrom(type);
    }
}