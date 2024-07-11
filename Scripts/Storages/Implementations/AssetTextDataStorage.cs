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

    public sealed class AssetTextDataStorage : IReadableDataStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetTextDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        public Type RawDataType => typeof(string);

        bool IDataStorage.CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type) && !typeof(IWritableData).IsAssignableFrom(type);

        object? IReadableDataStorage.Read(string key)
        {
            var text = this.assetsManager.Load<TextAsset>(key).text;
            this.assetsManager.Unload(key);
            return text.NullIfWhitespace();
        }

        #if UNIT_UNITASK
        UniTask<object?> IReadableDataStorage.ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)
                .ContinueWith(asset =>
                {
                    var text = asset.text;
                    this.assetsManager.Unload(key);
                    return (object?)text.NullIfWhitespace();
                });
        }
        #else
        IEnumerator IReadableDataStorage.ReadAsync(string key, Action<object?> callback, IProgress<float>? progress)
        {
            return this.assetsManager.LoadAsync<TextAsset>(
                key,
                asset =>
                {
                    var text = asset.text;
                    this.assetsManager.Unload(key);
                    callback(text.NullIfWhitespace());
                },
                progress
            );
        }
        #endif
    }
}