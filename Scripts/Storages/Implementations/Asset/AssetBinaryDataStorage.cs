#nullable enable
namespace TheOne.Data.Storage
{
    using System;
    using TheOne.Extensions;
    using TheOne.ResourceManagement;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if THEONE_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif
    #if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
    #endif

    public sealed class AssetBinaryDataStorage : EditorWritableDataStorage<byte[]>
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetBinaryDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        protected override byte[]? Read(string key)
        {
            var asset = this.assetsManager.Load<TextAsset>(key);
            var bytes = asset.bytes;
            this.assetsManager.Unload(key);
            return bytes.Length > 0 ? bytes : null;
        }

        protected override void Write(string key, byte[] value)
        {
            #if UNITY_EDITOR
            var asset = this.assetsManager.Load<TextAsset>(key);
            var path  = AssetDatabase.GetAssetPath(asset);
            this.assetsManager.Unload(key);
            File.WriteAllBytes(path, value);
            #endif
        }

        protected override void Flush()
        {
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
        }

        #if THEONE_UNITASK
        protected override async UniTask<byte[]?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            var asset = await this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken);
            var bytes = asset.bytes;
            this.assetsManager.Unload(key);
            return bytes.Length > 0 ? bytes : null;
        }

        protected override async UniTask WriteAsync(string key, byte[] value, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            #if UNITY_EDITOR
            var asset = await this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken);
            var path = AssetDatabase.GetAssetPath(asset);
            this.assetsManager.Unload(key);
            await File.WriteAllBytesAsync(path, value, cancellationToken);
            #endif
        }

        protected override UniTask FlushAsync(IProgress<float>? progress, CancellationToken cancellationToken)
        {
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
            return UniTask.CompletedTask;
        }
        #else
        protected override IEnumerator ReadAsync(string key, Action<byte[]?> callback, IProgress<float>? progress)
        {
            var asset = default(TextAsset)!;
            yield return this.assetsManager.LoadAsync<TextAsset>(key, result => asset = result, progress);
            var bytes = asset.bytes;
            this.assetsManager.Unload(key);
            callback(bytes.Length > 0 ? bytes : null);
        }

        protected override IEnumerator WriteAsync(string key, byte[] value, Action? callback, IProgress<float>? progress)
        {
            #if UNITY_EDITOR
            var asset = default(TextAsset)!;
            yield return this.assetsManager.LoadAsync<TextAsset>(key, result => asset = result, progress);
            var path = AssetDatabase.GetAssetPath(asset);
            this.assetsManager.Unload(key);
            yield return File.WriteAllBytesAsync(path, value).ToCoroutine();
            #endif
            callback?.Invoke();
        }

        protected override IEnumerator FlushAsync(Action? callback, IProgress<float>? progress)
        {
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
            callback?.Invoke();
            yield break;
        }
        #endif
    }
}