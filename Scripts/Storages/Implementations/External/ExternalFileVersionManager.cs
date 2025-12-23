#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Security.Cryptography;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class ExternalFileVersionManager : IExternalFileVersionManager
    {
        private static readonly string PERSISTENT_DATA_PATH = Application.persistentDataPath;
        private static readonly string TEMPORARY_CACHE_PATH = Application.temporaryCachePath;

        private readonly IExternalFileVersionManagerConfig config;
        private readonly ILogger                           logger;

        [Preserve]
        public ExternalFileVersionManager(IExternalFileVersionManagerConfig config, ILoggerManager loggerManager)
        {
            this.config = config;
            this.logger = loggerManager.GetLogger(this);
        }

        private string ZipFilePath      => $"{PERSISTENT_DATA_PATH}/{this.Version}";
        private string ExtractDirectory => $"{TEMPORARY_CACHE_PATH}/{this.Version}";

        private string Version
        {
            get => this.version;
            set
            {
                PlayerPrefs.SetString(nameof(ExternalFileVersionManager), this.version = value);
                PlayerPrefs.Save();
            }
        }

        private string version = PlayerPrefs.GetString(nameof(ExternalFileVersionManager));

        private bool validating;
        private bool validated;

        #region Sync

        string? IExternalFileVersionManager.GetFilePath(string name)
        {
            this.logger.Warning("`GetFilePath` only use cached file. Use `GetFilePathAsync` to download new file from remote.");
            this.ValidateAndExtract();
            if (!this.validated)
            {
                this.logger.Error("Failed to fetch version or download. Using cached data");
            }
            return this.GetFilePath(name);
        }

        private void ValidateAndExtract()
        {
            if (this.validated) return;
            this.logger.Debug($"Validating {this.Version}");

            if (this.Version.IsNullOrWhiteSpace())
            {
                this.logger.Error("Version not set");
                return;
            }

            if (!File.Exists(this.ZipFilePath))
            {
                this.logger.Error($"Zip file not found: {this.ZipFilePath}");
                return;
            }

            using var sha256  = SHA256.Create();
            using var zipFile = File.OpenRead(this.ZipFilePath);
            var       hash    = BitConverter.ToString(sha256.ComputeHash(zipFile)).Replace("-", "");
            if (!string.Equals(hash, this.Version, StringComparison.OrdinalIgnoreCase))
            {
                this.logger.Error($"Hash mismatch. Expected: {this.Version}, Got: {hash}");
                File.Delete(this.ZipFilePath);
                return;
            }

            this.logger.Debug($"Extracting {this.ZipFilePath} to {this.ExtractDirectory}");
            ZipFile.ExtractToDirectory(this.ZipFilePath, this.ExtractDirectory, true);

            this.logger.Debug("Validated");
            this.validated = true;
        }

        #endregion

        #region Async

        #if UNIT_UNITASK
        async UniTask<string?> IExternalFileVersionManager.GetFilePathAsync(string name, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            if (this.validating) await UniTask.WaitUntil(this, @this => !@this.validating, cancellationToken: cancellationToken);
            this.validating = true;
            try
            {
                var subProgresses = progress.CreateSubProgresses(2).ToArray();
                if (!this.validated)
                {
                    try
                    {
                        var version = await this.config.FetchVersionAsync(
                            progress: subProgresses[0],
                            cancellationToken: cancellationToken
                        );
                        if (!version.IsNullOrWhiteSpace())
                        {
                            this.Version = version.Trim();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        this.logger.Exception(e);
                    }
                    #if !UNITY_WEBGL
                    await UniTask.RunOnThreadPool(this.ValidateAndExtract, cancellationToken: cancellationToken);
                    #else
                    this.ValidateAndExtract();
                    #endif
                }
                if (!this.validated && !this.Version.IsNullOrWhiteSpace())
                {
                    try
                    {
                        await this.config.DownloadFileAsync(
                            version: this.Version,
                            savePath: this.ZipFilePath,
                            progress: subProgresses[1],
                            cancellationToken: cancellationToken
                        );
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        this.logger.Exception(e);
                    }
                    #if !UNITY_WEBGL
                    await UniTask.RunOnThreadPool(this.ValidateAndExtract, cancellationToken: cancellationToken);
                    #else
                    this.ValidateAndExtract();
                    #endif
                }
                if (!this.validated)
                {
                    this.logger.Error("Failed to fetch version or download. Using cached data");
                }
                return this.GetFilePath(name);
            }
            finally
            {
                this.validating = false;
            }
        }
        #else
        IEnumerator IExternalFileVersionManager.GetFilePathAsync(string name, Action<string?> callback, IProgress<float>? progress)
        {
            if (this.validating) yield return new WaitUntil(() => !this.validating);
            this.validating = true;
            try
            {
                var subProgresses = progress.CreateSubProgresses(2).ToArray();
                if (!this.validated)
                {
                    var version = default(string)!;
                    yield return this.config.FetchVersionAsync(
                        callback: result => version = result,
                        progress: subProgresses[0]
                    ).Catch(this.logger.Exception);
                    if (!version.IsNullOrWhiteSpace())
                    {
                        this.Version = version.Trim();
                    }
                    yield return CoroutineRunner.Run(this.ValidateAndExtract);
                }
                if (!this.validated && !this.Version.IsNullOrWhiteSpace())
                {
                    yield return this.config.DownloadFileAsync(
                        version: this.Version,
                        savePath: this.ZipFilePath,
                        progress: subProgresses[1]
                    ).Catch(this.logger.Exception);
                    yield return CoroutineRunner.Run(this.ValidateAndExtract);
                }
                if (!this.validated)
                {
                    this.logger.Error("Failed to fetch version or download. Using cached data");
                }
                callback(this.GetFilePath(name));
            }
            finally
            {
                this.validating = false;
            }
        }
        #endif

        #endregion

        private string? GetFilePath(string name)
        {
            var path = $"{this.ExtractDirectory}/{name}";
            return File.Exists(path) ? path : null;
        }
    }
}