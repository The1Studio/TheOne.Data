#nullable enable
namespace UniT.Data.Storage
{
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System;
    using System.Collections;
    #endif

    public interface IExternalFileVersionManager
    {
        public string GetFilePath(string name);

        #if UNIT_UNITASK
        public UniTask<string> GetFilePathAsync(string name, CancellationToken cancellationToken = default);
        #else
        public IEnumerator GetFilePathAsync(string name, Action<string> callback);
        #endif
    }

    public interface IExternalFileVersionManagerConfig
    {
        public string FetchVersionInfoUrl { get; }

        public string GetDownloadUrl(VersionInfo versionInfo);
    }

    public sealed class VersionInfo
    {
        public string Hash { get; }

        [Preserve]
        public VersionInfo(string hash)
        {
            this.Hash = hash;
        }

        public override string ToString()
        {
            return $"{nameof(VersionInfo)} {this.Hash}";
        }
    }
}