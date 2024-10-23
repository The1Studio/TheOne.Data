#nullable enable
namespace TheOne.Data.Storage
{
    using System;
    using System.IO;
    using UnityEngine.Scripting;
    #if THEONE_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    using TheOne.Extensions;
    #endif

    public sealed class ExternalBinaryDataStorage : DataStorage<byte[]>
    {
        private readonly IExternalFileVersionManager externalFileVersionManager;

        [Preserve]
        public ExternalBinaryDataStorage(IExternalFileVersionManager externalFileVersionManager)
        {
            this.externalFileVersionManager = externalFileVersionManager;
        }

        protected override byte[]? Read(string key)
        {
            return File.ReadAllBytes(this.externalFileVersionManager.GetFilePath(key));
        }

        #if THEONE_UNITASK
        protected override async UniTask<byte[]?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return await File.ReadAllBytesAsync(await this.externalFileVersionManager.GetFilePathAsync(key, cancellationToken), cancellationToken);
        }
        #else
        protected override IEnumerator ReadAsync(string key, Action<byte[]?> callback, IProgress<float>? progress)
        {
            return this.externalFileVersionManager.GetFilePathAsync(key, path => CoroutineRunner.Run(() => File.ReadAllBytes(path), callback));
        }
        #endif
    }
}