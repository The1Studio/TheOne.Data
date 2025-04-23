#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using UniT.Extensions;
    using UniT.ResourceManagement;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class AssetTextDataStorage : ReadOnlyDataStorage<string>
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetTextDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        protected override string? Read(string key)
        {
            var text = this.assetsManager.Load<TextAsset>(key).text;
            this.assetsManager.Unload(key);
            return text.NullIfWhiteSpace();
        }

        #if UNIT_UNITASK
        protected override UniTask<string?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)
                .ContinueWith(asset =>
                {
                    var text = asset.text;
                    this.assetsManager.Unload(key);
                    return text.NullIfWhiteSpace();
                });
        }
        #else
        protected override IEnumerator ReadAsync(string key, Action<string?> callback, IProgress<float>? progress)
        {
            return this.assetsManager.LoadAsync<TextAsset>(
                key,
                asset =>
                {
                    var text = asset.text;
                    this.assetsManager.Unload(key);
                    callback(text.NullIfWhiteSpace());
                },
                progress
            );
        }
        #endif
    }
}