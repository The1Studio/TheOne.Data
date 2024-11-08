#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Security.Cryptography;
    using Newtonsoft.Json;
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
    using UniT.Extensions;
    #endif

    public sealed class ExternalFileVersionManager : IExternalFileVersionManager
    {
        private const string VERSION_INFO_KEY = nameof(ExternalFileVersionManager) + "/" + nameof(VersionInfo);

        private static readonly string PersistentDataPath = Application.persistentDataPath;
        private static readonly string TemporaryCachePath = Application.temporaryCachePath;

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

        private string ZipFilePath      => $"{PersistentDataPath}/{this.versionInfo.Hash}";
        private string ExtractDirectory => $"{TemporaryCachePath}/{this.versionInfo.Hash}";

        private VersionInfo versionInfo = JsonConvert.DeserializeObject<VersionInfo>(PlayerPrefs.GetString(VERSION_INFO_KEY, "{}"))!;

        private bool fetching;
        private bool fetched;
        private bool validating;
        private bool validated;

        #region Sync

        string IExternalFileVersionManager.GetFilePath(string name)
        {
            this.logger.Warning("`GetFilePath` only use cached file. Use `GetFilePathAsync` to download new file from remote.");
            if (!this.ValidateAndExtract()) throw new Exception($"Could not validate {this.versionInfo}");
            return this.GetFilePath(name);
        }

        private bool ValidateAndExtract()
        {
            if (this.validated) return true;
            this.logger.Debug($"Validating {this.versionInfo}");
            if (!File.Exists(this.ZipFilePath))
            {
                this.logger.Error($"Zip file not found: {this.ZipFilePath}");
                PlayerPrefs.DeleteKey(VERSION_INFO_KEY);
                PlayerPrefs.Save();
                return false;
            }
            var hash = this.ComputeHash();
            if (hash != this.versionInfo.Hash)
            {
                this.logger.Error($"Hash mismatch. Expected: {this.versionInfo.Hash}, Got: {hash}");
                File.Delete(this.ZipFilePath);
                return false;
            }
            this.ExtractZipFile();
            this.logger.Debug("Validated");
            return this.validated = true;
        }

        #endregion

        #region Async

        #if UNIT_UNITASK
        async UniTask<string> IExternalFileVersionManager.GetFilePathAsync(string name, CancellationToken cancellationToken)
        {
            await this.FetchInfoAndDownloadAsync(cancellationToken);
            if (!await this.ValidateAndExtractAsync(cancellationToken)) throw new Exception($"Could not validate {this.versionInfo}");
            return this.GetFilePath(name);
        }

        private async UniTask<bool> ValidateAndExtractAsync(CancellationToken cancellationToken)
        {
            #if UNITY_WEBGL
            return this.ValidateAndExtract();
            #endif
            if (this.validating) await UniTask.WaitUntil(this, state => !state.validating, cancellationToken: cancellationToken);
            if (this.validated) return true;
            this.validating = true;
            try
            {
                this.logger.Debug($"Validating {this.versionInfo}");
                if (!File.Exists(this.ZipFilePath))
                {
                    this.logger.Error($"Zip file not found: {this.ZipFilePath}");
                    PlayerPrefs.DeleteKey(VERSION_INFO_KEY);
                    PlayerPrefs.Save();
                    return false;
                }
                var hash = await UniTask.RunOnThreadPool(this.ComputeHash, cancellationToken: cancellationToken);
                if (hash != this.versionInfo.Hash)
                {
                    this.logger.Error($"Hash mismatch. Expected: {this.versionInfo.Hash}, Got: {hash}");
                    File.Delete(this.ZipFilePath);
                    return false;
                }
                await UniTask.RunOnThreadPool(this.ExtractZipFile, cancellationToken: cancellationToken);
                this.logger.Debug("Validated");
                return this.validated = true;
            }
            finally
            {
                this.validating = false;
            }
        }

        private async UniTask FetchInfoAndDownloadAsync(CancellationToken cancellationToken)
        {
            if (this.fetching) await UniTask.WaitUntil(this, state => !state.fetching, cancellationToken: cancellationToken);
            if (this.fetched) return;
            var cancelled = false;
            this.fetching = true;
            try
            {
                var versionInfoStr = await this.externalAssetsManager.DownloadTextAsync(
                    url: this.config.FetchVersionInfoUrl,
                    cache: false,
                    cancellationToken: cancellationToken
                );
                this.versionInfo = JsonConvert.DeserializeObject<VersionInfo>(versionInfoStr)!;
                PlayerPrefs.SetString(VERSION_INFO_KEY, versionInfoStr);
                PlayerPrefs.Save();
                this.logger.Debug($"Got {this.versionInfo}");

                if (await this.ValidateAndExtractAsync(cancellationToken))
                {
                    this.logger.Debug("Skipping download");
                    return;
                }

                await this.externalAssetsManager.DownloadFileAsync(
                    url: this.config.GetDownloadUrl(this.versionInfo),
                    savePath: this.ZipFilePath,
                    cache: false,
                    cancellationToken: cancellationToken
                );
            }
            catch (OperationCanceledException)
            {
                cancelled = true;
                throw;
            }
            catch (Exception e)
            {
                this.HandleFetchAndDownloadException(e);
            }
            finally
            {
                this.fetching = false;
                this.fetched  = !cancelled;
            }
        }
        #else
        IEnumerator IExternalFileVersionManager.GetFilePathAsync(string name, Action<string> callback)
        {
            yield return this.FetchInfoAndDownloadAsync();
            yield return this.ValidateAndExtractAsync(validated =>
            {
                if (!validated) throw new Exception($"Could not validate {this.versionInfo}");
                callback(this.GetFilePath(name));
            });
        }

        private IEnumerator ValidateAndExtractAsync(Action<bool> callback)
        {
            #if UNITY_WEBGL
            return this.ValidateAndExtract();
            #endif
            if (this.validating) yield return new WaitUntil(() => !this.validating);
            if (this.validated)
            {
                callback(true);
                yield break;
            }
            this.validating = true;
            try
            {
                this.logger.Debug($"Validating {this.versionInfo}");
                if (!File.Exists(this.ZipFilePath))
                {
                    this.logger.Error($"Zip file not found: {this.ZipFilePath}");
                    PlayerPrefs.DeleteKey(VERSION_INFO_KEY);
                    PlayerPrefs.Save();
                    callback(false);
                    yield break;
                }
                var hash = default(string)!;
                yield return CoroutineRunner.Run(this.ComputeHash, result => hash = result);
                if (hash != this.versionInfo.Hash)
                {
                    this.logger.Error($"Hash mismatch. Expected: {this.versionInfo.Hash}, Got: {hash}");
                    File.Delete(this.ZipFilePath);
                    callback(false);
                    yield break;
                }
                yield return CoroutineRunner.Run(this.ExtractZipFile);
                this.logger.Debug("Validated");
                callback(this.validated = true);
            }
            finally
            {
                this.validating = false;
            }
        }

        private IEnumerator FetchInfoAndDownloadAsync()
        {
            if (this.fetching) yield return new WaitUntil(() => !this.fetching);
            if (this.fetched) yield break;
            this.fetching = true;
            try
            {
                var versionInfoStr = default(string)!;
                yield return this.externalAssetsManager.DownloadTextAsync(
                    url: this.config.FetchVersionInfoUrl,
                    callback: result => versionInfoStr = result,
                    cache: false
                ).Catch(this.HandleFetchAndDownloadException);
                this.versionInfo = JsonConvert.DeserializeObject<VersionInfo>(versionInfoStr)!;
                PlayerPrefs.SetString(VERSION_INFO_KEY, versionInfoStr);
                PlayerPrefs.Save();
                this.logger.Debug($"Got {this.versionInfo}");

                var validated = default(bool);
                yield return this.ValidateAndExtractAsync(result => validated = result);
                if (validated)
                {
                    this.logger.Debug("Skipping download");
                    yield break;
                }

                yield return this.externalAssetsManager.DownloadFileAsync(
                    url: this.config.GetDownloadUrl(this.versionInfo),
                    savePath: this.ZipFilePath,
                    cache: false
                ).Catch(this.HandleFetchAndDownloadException);
            }
            finally
            {
                this.fetching = false;
                this.fetched  = true;
            }
        }
        #endif

        #endregion

        private string GetFilePath(string name)
        {
            var path = $"{this.ExtractDirectory}/{name}";
            if (!File.Exists(path)) throw new FileNotFoundException($"File {name} not found", name);
            return path;
        }

        private string ComputeHash()
        {
            using var sha256  = SHA256.Create();
            using var zipFile = File.OpenRead(this.ZipFilePath);
            return BitConverter.ToString(sha256.ComputeHash(zipFile)).Replace("-", "");
        }

        private void ExtractZipFile()
        {
            this.logger.Debug($"Extracting {this.ZipFilePath} to {this.ExtractDirectory}");
            ZipFile.ExtractToDirectory(this.ZipFilePath, this.ExtractDirectory, true);
            this.logger.Debug("Extracted");
        }

        private void HandleFetchAndDownloadException(Exception e)
        {
            this.logger.Exception(e);
            this.logger.Error("Failed to fetch info or download. Using cached data");
        }
    }
}