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

    public sealed class ExternalBinaryDataStorage : DataStorage<byte[]>
    {
        private readonly IExternalFileVersionManager externalFileVersionManager;
        private readonly AssetBinaryDataStorage      assetBinaryDataStorage;

        [Preserve]
        public ExternalBinaryDataStorage(IExternalFileVersionManager externalFileVersionManager, AssetBinaryDataStorage assetBinaryDataStorage)
        {
            this.externalFileVersionManager = externalFileVersionManager;
            this.assetBinaryDataStorage     = assetBinaryDataStorage;
        }

        public override byte[]? Read(string key)
        {
            return this.externalFileVersionManager.GetFilePath(key) is { } path
                ? File.ReadAllBytes(path)
                : this.assetBinaryDataStorage.Read(key);
        }

        #if UNIT_UNITASK
        public override async UniTask<byte[]?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return await this.externalFileVersionManager.GetFilePathAsync(key, cancellationToken) is { } path
                ? await File.ReadAllBytesAsync(path, cancellationToken)
                : await this.assetBinaryDataStorage.ReadAsync(key, progress, cancellationToken);
        }
        #else
        public override IEnumerator ReadAsync(string key, Action<byte[]?> callback, IProgress<float>? progress)
        {
            var path = default(string);
            yield return this.externalFileVersionManager.GetFilePathAsync(key, result => path = result);
            yield return path is { }
                ? CoroutineRunner.Run(() => File.ReadAllBytes(path), callback)
                : this.assetBinaryDataStorage.ReadAsync(key, callback, progress);
        }
        #endif
    }
}