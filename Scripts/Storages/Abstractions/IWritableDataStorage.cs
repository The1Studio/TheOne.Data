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

    public interface IWritableDataStorage : IDataStorage
    {
        public void Write(string key, object value);

        public void Flush();

        #if THEONE_UNITASK
        public UniTask WriteAsync(string key, object value, IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask FlushAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator WriteAsync(string key, object value, Action? callback = null, IProgress<float>? progress = null);

        public IEnumerator FlushAsync(Action? callback = null, IProgress<float>? progress = null);
        #endif
    }
}