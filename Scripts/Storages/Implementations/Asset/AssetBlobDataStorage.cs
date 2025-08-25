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
    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    public sealed class AssetBlobDataStorage : EditorWritableDataStorage<Object>
    {
        private readonly IAssetsManager assetsManager;

        [Preserve]
        public AssetBlobDataStorage(IAssetsManager assetsManager)
        {
            this.assetsManager = assetsManager;
        }

        protected override Object? Read(string key)
        {
            return this.assetsManager.Load<Object>(key);
        }

        protected override void Write(string key, Object value)
        {
        }

        protected override void Flush()
        {
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
        }

        #if UNIT_UNITASK
        protected override UniTask<Object?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return this.assetsManager.LoadAsync<Object>(key, progress, cancellationToken)!;
        }

        protected override UniTask WriteAsync(string key, Object value, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask FlushAsync(IProgress<float>? progress, CancellationToken cancellationToken)
        {
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
            return UniTask.CompletedTask;
        }
        #else
        protected override IEnumerator ReadAsync(string key, Action<Object?> callback, IProgress<float>? progress)
        {
            return this.assetsManager.LoadAsync<Object>(key, callback, progress);
        }

        protected override IEnumerator WriteAsync(string key, Object value, Action? callback, IProgress<float>? progress)
        {
            yield break;
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