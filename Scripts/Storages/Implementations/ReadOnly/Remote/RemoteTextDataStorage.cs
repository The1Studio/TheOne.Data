#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using System.IO;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    using UniT.Extensions;
    #endif

    public sealed class RemoteTextDataStorage : ReadOnlyDataStorage<string>
    {
        private readonly IRemoteFileVersionManager remoteFileVersionManager;

        [Preserve]
        public RemoteTextDataStorage(IRemoteFileVersionManager remoteFileVersionManager)
        {
            this.remoteFileVersionManager = remoteFileVersionManager;
        }

        protected override string? Read(string key)
        {
            return File.ReadAllText(this.remoteFileVersionManager.GetFilePath(key));
        }

        #if UNIT_UNITASK
        protected override async UniTask<string?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return await File.ReadAllTextAsync(await this.remoteFileVersionManager.GetFilePathAsync(key, cancellationToken), cancellationToken);
        }
        #else
        protected override IEnumerator ReadAsync(string key, Action<string?> callback, IProgress<float>? progress)
        {
            return this.remoteFileVersionManager.GetFilePathAsync(key, path => CoroutineRunner.Run(() => File.ReadAllText(path), callback));
        }
        #endif
    }
}