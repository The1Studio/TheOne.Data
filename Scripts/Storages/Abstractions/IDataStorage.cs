#nullable enable
namespace TheOne.Data.Storage
{
    using System;
    #if THEONE_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IDataStorage
    {
        public Type RawDataType { get; }

        public bool CanStore(Type type);

        public object? Read(string key);

        #if THEONE_UNITASK
        public UniTask<object?> ReadAsync(string key, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator ReadAsync(string key, Action<object?> callback, IProgress<float>? progress = null);
        #endif
    }
}