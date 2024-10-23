#nullable enable
namespace TheOne.Data.Storage
{
    using System;
    #if THEONE_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IExternalFileVersionManager
    {
        public string? GetFilePath(string name);

        #if THEONE_UNITASK
        public UniTask<string?> GetFilePathAsync(string name, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator GetFilePathAsync(string name, Action<string?> callback, IProgress<float>? progress = null);
        #endif
    }

    public interface IExternalFileVersionManagerConfig
    {
        #if THEONE_UNITASK
        public UniTask<string> FetchVersionAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask DownloadFileAsync(string version, string savePath, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator FetchVersionAsync(Action<string> callback, IProgress<float>? progress = null);

        public IEnumerator DownloadFileAsync(string version, string savePath, Action? callback = null, IProgress<float>? progress = null);
        #endif
    }
}