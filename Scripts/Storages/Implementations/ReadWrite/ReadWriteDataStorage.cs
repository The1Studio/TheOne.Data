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

    public abstract class ReadWriteDataStorage<TRawData> : DataStorage<TRawData>, IReadableDataStorage, IWritableDataStorage
    {
        protected override bool CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type) && typeof(IWritableData).IsAssignableFrom(type);

        #region Sync

        object? IReadableDataStorage.Read(string key) => this.Read(key);

        void IWritableDataStorage.Write(string key, object value) => this.Write(key, (TRawData)value);

        void IWritableDataStorage.Flush() => this.Flush();

        protected abstract TRawData? Read(string key);

        protected abstract void Write(string key, TRawData value);

        protected abstract void Flush();

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask<object?> IReadableDataStorage.ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken) => this.ReadAsync(key, progress, cancellationToken).ContinueWith(rawData => (object?)rawData);

        UniTask IWritableDataStorage.WriteAsync(string key, object value, IProgress<float>? progress, CancellationToken cancellationToken) => this.WriteAsync(key, (TRawData)value, progress, cancellationToken);

        UniTask IWritableDataStorage.FlushAsync(IProgress<float>? progress, CancellationToken cancellationToken) => this.FlushAsync(progress, cancellationToken);

        protected abstract UniTask<TRawData?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken);

        protected abstract UniTask WriteAsync(string key, TRawData value, IProgress<float>? progress, CancellationToken cancellationToken);

        protected abstract UniTask FlushAsync(IProgress<float>? progress, CancellationToken cancellationToken);
        #else
        IEnumerator IReadableDataStorage.ReadAsync(string key, Action<object?> callback, IProgress<float>? progress) => this.ReadAsync(key, rawData => callback(rawData), progress);

        IEnumerator IWritableDataStorage.WriteAsync(string key, object value, Action? callback, IProgress<float>? progress) => this.WriteAsync(key, (TRawData)value, callback, progress);

        IEnumerator IWritableDataStorage.FlushAsync(Action? callback, IProgress<float>? progress) => this.FlushAsync(callback, progress);

        protected abstract IEnumerator ReadAsync(string key, Action<TRawData?> callback, IProgress<float>? progress);

        protected abstract IEnumerator WriteAsync(string key, TRawData value, Action? callback, IProgress<float>? progress);

        protected abstract IEnumerator FlushAsync(Action? callback, IProgress<float>? progress);
        #endif

        #endregion
    }
}