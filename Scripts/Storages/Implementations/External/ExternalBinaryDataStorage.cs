#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using System.IO;
    using UniT.Logging;
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
        private readonly ILogger                     logger;

        [Preserve]
        public ExternalBinaryDataStorage(IExternalFileVersionManager externalFileVersionManager, AssetBinaryDataStorage assetBinaryDataStorage, ILoggerManager loggerManager)
        {
            this.externalFileVersionManager = externalFileVersionManager;
            this.assetBinaryDataStorage     = assetBinaryDataStorage;
            this.logger                     = loggerManager.GetLogger(this);
        }

        public override byte[]? Read(string key)
        {
            var path = this.externalFileVersionManager.GetFilePath(key);
            if (path is null)
            {
                this.logger.Warning($"{key} not found, fallback to local asset");
                return this.assetBinaryDataStorage.Read(key);
            }
            return File.ReadAllBytes(path);
        }

        #if UNIT_UNITASK
        public override async UniTask<byte[]?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            var path = await this.externalFileVersionManager.GetFilePathAsync(key, cancellationToken);
            if (path is null)
            {
                this.logger.Warning($"{key} not found, fallback to local asset");
                return await this.assetBinaryDataStorage.ReadAsync(key, progress, cancellationToken);
            }
            return await File.ReadAllBytesAsync(path, cancellationToken);
        }
        #else
        public override IEnumerator ReadAsync(string key, Action<byte[]?> callback, IProgress<float>? progress)
        {
            var path = default(string);
            yield return this.externalFileVersionManager.GetFilePathAsync(key, result => path = result);
            if (path is null)
            {
                this.logger.Warning($"{key} not found, fallback to local asset");
                yield return this.assetBinaryDataStorage.ReadAsync(key, callback, progress);
                yield break;
            }
            yield return File.ReadAllBytesAsync(path).ToCoroutine(callback);
        }
        #endif
    }
}