#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using UniT.ResourceManagement;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class AssetBlobDataStorage : IReadableDataStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetBlobDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        public Type RawDataType => typeof(Object);

        bool IDataStorage.CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type) && !typeof(IWritableData).IsAssignableFrom(type);

        object IReadableDataStorage.Read(string key)
        {
            return this.assetsManager.Load<Object>(key);
        }

        #if UNIT_UNITASK
        UniTask<object?> IReadableDataStorage.ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return this.assetsManager.LoadAsync<Object>(key, progress, cancellationToken)
                .ContinueWith(asset => (object?)asset);
        }
        #else
        IEnumerator IReadableDataStorage.ReadAsync(string key, Action<object> callback, IProgress<float>? progress)
        {
            return this.assetsManager.LoadAsync<Object>(key, callback, progress);
        }
        #endif
    }
}