#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Data.Serialization;
    using UniT.Data.Storage;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class DataManager : IDataManager
    {
        #region Constructor

        private readonly IReadOnlyList<(ISerializer Serializer, IDataStorage Storage)> serializersAndStorages;
        private readonly ILogger                                                       logger;

        private readonly Dictionary<string, IData>                                        dataCache                 = new Dictionary<string, IData>();
        private readonly Dictionary<Type, (ISerializer Serializer, IDataStorage Storage)> serializerAndStorageCache = new Dictionary<Type, (ISerializer, IDataStorage)>();

        [Preserve]
        public DataManager(IEnumerable<ISerializer> serializers, IEnumerable<IDataStorage> storages, ILoggerManager loggerManager)
        {
            this.serializersAndStorages = IterTools.Product(serializers, storages)
                .Where((serializer, storage) => serializer.RawDataType == storage.RawDataType)
                .ToArray();
            this.logger = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Sync

        IData[] IDataManager.Load(string[] keys, Type[] types) => this.Load(keys, types);

        void IDataManager.Save(string[] keys) => this.Save(keys);

        void IDataManager.Flush(string[] keys) => this.Flush(keys);

        void IDataManager.SaveAll() => this.Save(this.WritableDataKeys);

        void IDataManager.FlushAll() => this.Flush(this.WritableDataKeys);

        private IData[] Load(string[] keys, Type[] types)
        {
            return IterTools.Zip(keys, types)
                .Select((key, type) => this.dataCache.GetOrAdd(key, () =>
                {
                    var (serializer, storage) = this.GetSerializerAndStorage(type);
                    if (storage is not IReadableDataStorage readableStorage) throw new InvalidOperationException($"{key} is not readable");
                    var rawData = readableStorage.Read(key);
                    if (rawData is null)
                    {
                        var data = (IData)type.GetEmptyConstructor()();
                        this.logger.Debug($"Instantiated {key}");
                        return data;
                    }
                    else
                    {
                        var data = serializer.Deserialize(type, rawData);
                        this.logger.Debug($"Loaded {key}");
                        return data;
                    }
                }))
                .ToArray();
        }

        private void Save(IEnumerable<string> keys)
        {
            keys.ForEach(key =>
            {
                if (!this.dataCache.TryGetValue(key, out var data))
                {
                    this.logger.Warning($"Trying to save {key} that was not loaded");
                    return;
                }
                var (serializer, storage) = this.GetSerializerAndStorage(data.GetType());
                if (storage is not IWritableDataStorage writableStorage) throw new InvalidOperationException($"{key} is not writable");
                var rawData = serializer.Serialize(data);
                writableStorage.Write(key, rawData);
                this.logger.Debug($"Saved {key}");
            });
        }

        private void Flush(IEnumerable<string> keys)
        {
            keys.Where(this.dataCache.ContainsKey)
                .Select(key => this.GetSerializerAndStorage(this.dataCache[key].GetType()).Storage)
                .Distinct()
                .ForEach(storage =>
                {
                    if (storage is not IWritableDataStorage writableStorage) throw new InvalidOperationException();
                    writableStorage.Flush();
                });
        }

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask<IData[]> IDataManager.LoadAsync(string[] keys, Type[] types, IProgress<float>? progress, CancellationToken cancellationToken) => this.LoadAsync(keys, types, progress, cancellationToken);

        UniTask IDataManager.SaveAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken) => this.SaveAsync(keys, progress, cancellationToken);

        UniTask IDataManager.FlushAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken) => this.FlushAsync(keys, progress, cancellationToken);

        UniTask IDataManager.SaveAllAsync(IProgress<float>? progress, CancellationToken cancellationToken) => this.SaveAsync(this.WritableDataKeys, progress, cancellationToken);

        UniTask IDataManager.FlushAllAsync(IProgress<float>? progress, CancellationToken cancellationToken) => this.FlushAsync(this.WritableDataKeys, progress, cancellationToken);

        private UniTask<IData[]> LoadAsync(string[] keys, Type[] types, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return IterTools.Zip(keys, types).SelectAsync(
                (key_type, progress, cancellationToken) => this.dataCache.GetOrAddAsync(key_type.Item1, async () =>
                {
                    var (key, type)           = key_type;
                    var (serializer, storage) = this.GetSerializerAndStorage(type);
                    if (storage is not IReadableDataStorage readableStorage) throw new InvalidOperationException($"{key} is not readable");
                    var rawData = await readableStorage.ReadAsync(key, progress, cancellationToken);
                    if (rawData is null)
                    {
                        var data = (IData)type.GetEmptyConstructor()();
                        this.logger.Debug($"Instantiated {key}");
                        return data;
                    }
                    else
                    {
                        var data = await serializer.DeserializeAsync(type, rawData, cancellationToken);
                        this.logger.Debug($"Loaded {key}");
                        return data;
                    }
                }),
                progress,
                cancellationToken
            ).ToArrayAsync();
        }

        private UniTask SaveAsync(IEnumerable<string> keys, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return keys.ForEachAsync(
                async (key, progress, cancellationToken) =>
                {
                    if (!this.dataCache.TryGetValue(key, out var data))
                    {
                        this.logger.Warning($"Trying to save {key} that was not loaded");
                        return;
                    }
                    var (serializer, storage) = this.GetSerializerAndStorage(data.GetType());
                    if (storage is not IWritableDataStorage writableStorage) throw new InvalidOperationException($"{key} is not writable");
                    var rawData = await serializer.SerializeAsync(data, cancellationToken);
                    await writableStorage.WriteAsync(key, rawData, progress, cancellationToken);
                    this.logger.Debug($"Saved {key}");
                },
                progress,
                cancellationToken
            );
        }

        private UniTask FlushAsync(IEnumerable<string> keys, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return keys.Where(this.dataCache.ContainsKey)
                .Select(key => this.GetSerializerAndStorage(this.dataCache[key].GetType()).Storage)
                .Distinct()
                .ForEachAsync(
                    async (storage, progress, cancellationToken) =>
                    {
                        if (storage is not IWritableDataStorage writableStorage) throw new InvalidOperationException();
                        await writableStorage.FlushAsync(progress, cancellationToken);
                    },
                    progress,
                    cancellationToken
                );
        }
        #else
        IEnumerator IDataManager.LoadAsync(string[] keys, Type[] types, Action<IData[]> callback, IProgress<float>? progress) => this.LoadAsync(keys, types, callback, progress);

        IEnumerator IDataManager.SaveAsync(string[] keys, Action? callback, IProgress<float>? progress) => this.SaveAsync(keys, callback, progress);

        IEnumerator IDataManager.FlushAsync(string[] keys, Action? callback, IProgress<float>? progress) => this.FlushAsync(keys, callback, progress);

        IEnumerator IDataManager.SaveAllAsync(Action? callback, IProgress<float>? progress) => this.SaveAsync(this.WritableDataKeys, callback, progress);

        IEnumerator IDataManager.FlushAllAsync(Action? callback, IProgress<float>? progress) => this.FlushAsync(this.WritableDataKeys, callback, progress);

        private IEnumerator LoadAsync(string[] keys, Type[] types, Action<IData[]> callback, IProgress<float>? progress)
        {
            return IterTools.Zip(keys, types).SelectAsync<(string, Type), IData>(
                (key_type, callback, progress) => this.dataCache.GetOrAddAsync(key_type.Item1, callback => LoadAsync(key_type.Item1, key_type.Item2, callback, progress), callback),
                datas => callback(datas.ToArray()),
                progress
            );

            IEnumerator LoadAsync(string key, Type type, Action<IData> callback, IProgress<float>? progress)
            {
                var (serializer, storage) = this.GetSerializerAndStorage(type);
                if (storage is not IReadableDataStorage readableStorage) throw new InvalidOperationException($"{key} is not readable");
                var rawData = default(object);
                yield return readableStorage.ReadAsync(key, result => rawData = result, progress);
                if (rawData is null)
                {
                    callback((IData)type.GetEmptyConstructor()());
                    this.logger.Debug($"Instantiated {key}");
                }
                else
                {
                    yield return serializer.DeserializeAsync(type, rawData, callback);
                    this.logger.Debug($"Loaded {key}");
                }
            }
        }

        private IEnumerator SaveAsync(IEnumerable<string> keys, Action? callback, IProgress<float>? progress)
        {
            return keys.ForEachAsync(SaveAsync, callback, progress);

            IEnumerator SaveAsync(string key, IProgress<float>? progress)
            {
                if (!this.dataCache.TryGetValue(key, out var data))
                {
                    this.logger.Warning($"Trying to save {key} that was not loaded");
                    yield break;
                }
                var (serializer, storage) = this.GetSerializerAndStorage(data.GetType());
                if (storage is not IWritableDataStorage writableStorage) throw new InvalidOperationException($"{key} is not writable");
                var rawData = default(object)!;
                yield return serializer.SerializeAsync(data, result => rawData = result);
                yield return writableStorage.WriteAsync(key, rawData, progress: progress);
                this.logger.Debug($"Saved {key}");
            }
        }

        private IEnumerator FlushAsync(IEnumerable<string> keys, Action? callback, IProgress<float>? progress)
        {
            return keys.Where(this.dataCache.ContainsKey)
                .Select(key => this.GetSerializerAndStorage(this.dataCache[key].GetType()).Storage)
                .Distinct()
                .ForEachAsync(FlushAsync, callback, progress);

            IEnumerator FlushAsync(IDataStorage storage, IProgress<float>? progress)
            {
                if (storage is not IWritableDataStorage writableStorage) throw new InvalidOperationException();
                yield return writableStorage.FlushAsync(progress: progress);
            }
        }
        #endif

        #endregion

        #region Private

        private IEnumerable<string> WritableDataKeys => this.dataCache.Where<string, IData>((_, data) => data is IWritableData).Select<string, IData, string>((key, _) => key);

        private (ISerializer Serializer, IDataStorage Storage) GetSerializerAndStorage(Type type)
        {
            return this.serializerAndStorageCache.GetOrAdd(type, () =>
            {
                var serializersAndStorages = this.serializersAndStorages.Where((serializer, storage) => serializer.CanSerialize(type) && storage.CanStore(type)).ToArray();
                if (serializersAndStorages.Length is 0) throw new InvalidOperationException($"No serializer or storage found for {type.Name}");
                return serializersAndStorages[^1];
            });
        }

        #endregion
    }
}