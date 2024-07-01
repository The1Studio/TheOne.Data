#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using System.Linq;
    using UniT.Extensions;
    using UniT.ResourceManagement;
    using UnityEngine;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class AssetDataStorage : IReadableBinaryStorage, IReadableStringStorage, IReadableObjectStorage
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        bool IDataStorage.CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type) && !typeof(IWritableData).IsAssignableFrom(type);

        byte[][] IReadableBinaryStorage.Read(string[] keys)
        {
            return keys.Select(key =>
            {
                var bytes = this.assetsManager.Load<TextAsset>(key).bytes;
                this.assetsManager.Unload(key);
                return bytes;
            }).ToArray();
        }

        string[] IReadableStringStorage.Read(string[] keys)
        {
            return keys.Select(key =>
            {
                var text = this.assetsManager.Load<TextAsset>(key).text;
                this.assetsManager.Unload(key);
                return text;
            }).ToArray();
        }

        object[] IReadableObjectStorage.Read(string[] keys)
        {
            return keys.Select(key =>
            {
                var data = this.assetsManager.Load<Object>(key);
                return (object)data;
            }).ToArray();
        }

        #if UNIT_UNITASK
        UniTask<byte[][]> IReadableBinaryStorage.ReadAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return keys.SelectAsync(
                (key, progress, cancellationToken) =>
                    this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)
                        .ContinueWith(asset =>
                        {
                            var bytes = asset.bytes;
                            this.assetsManager.Unload(key);
                            return bytes;
                        }),
                progress,
                cancellationToken
            ).ToArrayAsync();
        }

        UniTask<string[]> IReadableStringStorage.ReadAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return keys.SelectAsync(
                (key, progress, cancellationToken) =>
                    this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)
                        .ContinueWith(asset =>
                        {
                            var text = asset.text;
                            this.assetsManager.Unload(key);
                            return text;
                        }),
                progress,
                cancellationToken
            ).ToArrayAsync();
        }

        UniTask<object[]> IReadableObjectStorage.ReadAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return keys.SelectAsync(
                (key, progress, cancellationToken) =>
                    this.assetsManager.LoadAsync<Object>(key, progress, cancellationToken)
                        .ContinueWith(asset => (object)asset),
                progress,
                cancellationToken
            ).ToArrayAsync();
        }
        #else
        IEnumerator IReadableBinaryStorage.ReadAsync(string[] keys, Action<byte[][]> callback, IProgress<float>? progress)
        {
            return keys.SelectAsync<string, byte[]>(
                (key, callback, progress) => this.assetsManager.LoadAsync<TextAsset>(key, asset => callback(asset.bytes), progress),
                rawDatas => callback(rawDatas.ToArray()),
                progress
            );
        }

        IEnumerator IReadableStringStorage.ReadAsync(string[] keys, Action<string[]> callback, IProgress<float>? progress)
        {
            return keys.SelectAsync<string, string>(
                (key, callback, progress) => this.assetsManager.LoadAsync<TextAsset>(key, asset => callback(asset.text), progress),
                rawDatas => callback(rawDatas.ToArray()),
                progress
            );
        }

        IEnumerator IReadableObjectStorage.ReadAsync(string[] keys, Action<object[]> callback, IProgress<float>? progress)
        {
            return keys.SelectAsync<string, object>(
                this.assetsManager.LoadAsync<TextAsset>,
                rawDatas => callback(rawDatas.ToArray()),
                progress
            );
        }
        #endif
    }
}