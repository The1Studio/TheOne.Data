#nullable enable
namespace UniT.Data.Storage
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public abstract class ReadOnlyDataStorage<TRawData> : DataStorage<TRawData>, IReadableDataStorage
    {
        protected override bool CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type) && !typeof(IWritableData).IsAssignableFrom(type);

        #region Sync

        object? IReadableDataStorage.Read(string key) => this.Read(key);

        protected abstract TRawData? Read(string key);

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask<object?> IReadableDataStorage.ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken) => this.ReadAsync(key, progress, cancellationToken).ContinueWith(rawData => (object?)rawData);

        protected abstract UniTask<TRawData?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken);
        #else
        IEnumerator IReadableDataStorage.ReadAsync(string key, Action<object?> callback, IProgress<float>? progress) => this.ReadAsync(key, rawData => callback(rawData), progress);

        protected abstract IEnumerator ReadAsync(string key, Action<TRawData?> callback, IProgress<float>? progress);
        #endif

        #endregion
    }
}