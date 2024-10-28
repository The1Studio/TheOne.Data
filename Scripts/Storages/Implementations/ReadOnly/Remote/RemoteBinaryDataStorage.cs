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

    public sealed class RemoteBinaryDataStorage : ReadOnlyDataStorage<byte[]>
    {
        private readonly IRemoteFileVersionManager remoteFileVersionManager;

        [Preserve]
        public RemoteBinaryDataStorage(IRemoteFileVersionManager remoteFileVersionManager)
        {
            this.remoteFileVersionManager = remoteFileVersionManager;
        }

        protected override byte[]? Read(string key)
        {
            return File.ReadAllBytes(this.remoteFileVersionManager.GetFilePath(key));
        }

        #if UNIT_UNITASK
        protected override async UniTask<byte[]?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return await File.ReadAllBytesAsync(await this.remoteFileVersionManager.GetFilePathAsync(key, cancellationToken), cancellationToken);
        }
        #else
        protected override IEnumerator ReadAsync(string key, Action<byte[]?> callback, IProgress<float>? progress)
        {
            return this.remoteFileVersionManager.GetFilePathAsync(key, path => CoroutineRunner.Run(() => File.ReadAllBytes(path), callback));
        }
        #endif
    }
}