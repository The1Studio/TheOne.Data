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

    public sealed class ExternalTextDataStorage : DataStorage<string>
    {
        private readonly IExternalFileVersionManager externalFileVersionManager;

        [Preserve]
        public ExternalTextDataStorage(IExternalFileVersionManager externalFileVersionManager)
        {
            this.externalFileVersionManager = externalFileVersionManager;
        }

        protected override string? Read(string key)
        {
            return File.ReadAllText(this.externalFileVersionManager.GetFilePath(key));
        }

        #if UNIT_UNITASK
        protected override async UniTask<string?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return await File.ReadAllTextAsync(await this.externalFileVersionManager.GetFilePathAsync(key, cancellationToken), cancellationToken);
        }
        #else
        protected override IEnumerator ReadAsync(string key, Action<string?> callback, IProgress<float>? progress)
        {
            return this.externalFileVersionManager.GetFilePathAsync(key, path => CoroutineRunner.Run(() => File.ReadAllText(path), callback));
        }
        #endif
    }
}