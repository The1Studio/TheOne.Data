#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Security.Cryptography;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.ResourceManagement;
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
        private readonly IExternalFileVersionManagerConfig config;
        private readonly IExternalAssetsManager            externalAssetsManager;
        private readonly ILogger                           logger;

        [Preserve]
        public ExternalFileVersionManager(IExternalFileVersionManagerConfig config, IExternalAssetsManager externalAssetsManager, ILoggerManager loggerManager)
        {
            this.config                = config;
            this.externalAssetsManager = externalAssetsManager;
            this.logger                = loggerManager.GetLogger(this);
        }

        private string ZipFilePath      => $"{Application.persistentDataPath}/{this.Version}";
        private string ExtractDirectory => $"{Application.temporaryCachePath}/{this.Version}";

        private string version = PlayerPrefs.GetString(nameof(ExternalFileVersionManager));

        private string Version
        {
            get => this.version;
            set
            {
                PlayerPrefs.SetString(nameof(ExternalFileVersionManager), this.version = value);
                PlayerPrefs.Save();
            }
        }

        private bool validating;
        private bool validated;

        #region Sync

        string? IExternalFileVersionManager.GetFilePath(string name)
        {
            this.logger.Warning("`GetFilePath` only use cached file. Use `GetFilePathAsync` to download new file from remote.");
            this.ValidateAndExtract();
            return this.validated ? this.GetFilePath(name) : null;
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
            if (hash != this.Version)
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
        async UniTask<string?> IExternalFileVersionManager.GetFilePathAsync(string name, CancellationToken cancellationToken)
        {
            if (this.validating) await UniTask.WaitUntil(this, @this => !@this.validating, cancellationToken: cancellationToken);
            this.validating = true;
            try
            {
                if (!this.validated)
                {
                    try
                    {
                        this.Version = await this.externalAssetsManager.DownloadTextAsync(
                            url: this.config.FetchVersionUrl,
                            cache: false,
                            cancellationToken: cancellationToken
                        );
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        this.HandleNonCriticalException(e);
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
                        await this.externalAssetsManager.DownloadFileAsync(
                            url: this.config.GetDownloadUrl(this.Version),
                            savePath: this.ZipFilePath,
                            cache: false,
                            cancellationToken: cancellationToken
                        );
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        this.HandleNonCriticalException(e);
                    }
                    #if !UNITY_WEBGL
                    await UniTask.RunOnThreadPool(this.ValidateAndExtract, cancellationToken: cancellationToken);
                    #else
                    this.ValidateAndExtract();
                    #endif
                }
                return this.validated ? this.GetFilePath(name) : null;
            }
            finally
            {
                this.validating = false;
            }
        }
        #else
        IEnumerator IExternalFileVersionManager.GetFilePathAsync(string name, Action<string?> callback)
        {
            if (this.validating) yield return new WaitUntil(() => !this.validating);
            this.validating = true;
            try
            {
                if (!this.validated)
                {
                    yield return this.externalAssetsManager.DownloadTextAsync(
                        url: this.config.FetchVersionUrl,
                        callback: result => this.Version = result,
                        cache: false
                    ).Catch(this.HandleNonCriticalException);
                    yield return CoroutineRunner.Run(this.ValidateAndExtract);
                }
                if (!this.validated && !this.Version.IsNullOrWhiteSpace())
                {
                    yield return this.externalAssetsManager.DownloadFileAsync(
                        url: this.config.GetDownloadUrl(this.Version),
                        savePath: this.ZipFilePath,
                        cache: false
                    ).Catch(this.HandleNonCriticalException);
                    yield return CoroutineRunner.Run(this.ValidateAndExtract);
                }
                callback(this.validated ? this.GetFilePath(name) : null);
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

        private void HandleNonCriticalException(Exception e)
        {
            this.logger.Exception(e);
            this.logger.Error("Failed to fetch info or download. Using cached data");
        }
    }
}