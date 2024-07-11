#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using UniT.ResourceManagement;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class AssetBinaryDataStorage : IReadableDataStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetBinaryDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        public Type RawDataType => typeof(byte[]);

        bool IDataStorage.CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type) && !typeof(IWritableData).IsAssignableFrom(type);

        object? IReadableDataStorage.Read(string key)
        {
            var bytes = this.assetsManager.Load<TextAsset>(key).bytes;
            this.assetsManager.Unload(key);
            return bytes.Length > 0 ? bytes : default(object);
        }

        #if UNIT_UNITASK
        UniTask<object?> IReadableDataStorage.ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)
                .ContinueWith(asset =>
                {
                    var bytes = asset.bytes;
                    this.assetsManager.Unload(key);
                    return bytes.Length > 0 ? bytes : default(object);
                });
        }
        #else
        IEnumerator IReadableDataStorage.ReadAsync(string key, Action<object?> callback, IProgress<float>? progress)
        {
            return this.assetsManager.LoadAsync<TextAsset>(
                key,
                asset =>
                {
                    var bytes = asset.bytes;
                    this.assetsManager.Unload(key);
                    callback(bytes.Length > 0 ? bytes : default(object));
                },
                progress
            );
        }
        #endif
    }
}