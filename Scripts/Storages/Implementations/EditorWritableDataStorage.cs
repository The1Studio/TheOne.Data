#nullable enable
namespace UniT.Data.Storage
{
    using System;

    public abstract class EditorWritableDataStorage<TRawData> : WritableDataStorage<TRawData>, IEditorWritableDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IEditorWritableData).IsAssignableFrom(type);
    }
}