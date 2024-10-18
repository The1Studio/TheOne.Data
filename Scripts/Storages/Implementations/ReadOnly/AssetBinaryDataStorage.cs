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

    public class AssetBinaryDataStorage : ReadOnlyDataStorage<byte[]>
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetBinaryDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        protected override byte[]? Read(string key)
        {
            var bytes = this.assetsManager.Load<TextAsset>(key).bytes;
            this.assetsManager.Unload(key);
            return bytes.Length > 0 ? bytes : null;
        }

        #if UNIT_UNITASK
        protected override UniTask<byte[]?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)
                .ContinueWith(asset =>
                {
                    var bytes = asset.bytes;
                    this.assetsManager.Unload(key);
                    return bytes.Length > 0 ? bytes : null;
                });
        }
        #else
        protected override IEnumerator ReadAsync(string key, Action<byte[]?> callback, IProgress<float>? progress)
        {
            return this.assetsManager.LoadAsync<TextAsset>(
                key,
                asset =>
                {
                    var bytes = asset.bytes;
                    this.assetsManager.Unload(key);
                    callback(bytes.Length > 0 ? bytes : null);
                },
                progress
            );
        }
        #endif
    }
}