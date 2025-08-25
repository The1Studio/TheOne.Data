#nullable enable
namespace TheOne.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TheOne.Data.Serialization;
    using TheOne.Data.Storage;
    using TheOne.Extensions;
    using TheOne.Logging;
    using UnityEngine.Scripting;
    #if THEONE_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class DataManager : IDataManager
    {
        #region Constructor

        private readonly IReadOnlyList<ISerializer>  serializers;
        private readonly IReadOnlyList<IDataStorage> storages;
        private readonly ILogger                     logger;

        private readonly Dictionary<string, object>                                       dataCache                 = new Dictionary<string, object>();
        private readonly Dictionary<Type, (ISerializer Serializer, IDataStorage Storage)> serializerAndStorageCache = new Dictionary<Type, (ISerializer, IDataStorage)>();

        [Preserve]
        public DataManager(IEnumerable<ISerializer> serializers, IEnumerable<IDataStorage> storages, ILoggerManager loggerManager)
        {
            this.serializers = serializers.ToArray();
            this.storages    = storages.ToArray();
            this.logger      = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Sync

        object[] IDataManager.Load(string[] keys, Type[] types, bool cache) => this.Load(keys, types, cache);

        void IDataManager.Update(string[] keys, object[] datas) => this.Update(keys, datas);

        void IDataManager.Save(string[] keys) => this.Save(keys);

        void IDataManager.Flush(string[] keys) => this.Flush(keys);

        void IDataManager.SaveAll() => this.Save(this.WritableKeys);

        void IDataManager.FlushAll() => this.Flush(this.WritableKeys);

        private object[] Load(string[] keys, Type[] types, bool cache)
        {
            return IterTools.Zip(keys, types)
                .Select((key, type) => cache
                    ? this.dataCache.GetOrAdd(key, () => Load(key, type))
                    : Load(key, type)
                )
                .ToArray();

            object Load(string key, Type type)
            {
                var (serializer, storage) = this.GetSerializerAndStorage(type);
                var rawData = storage.Read(key);
                if (rawData is null)
                {
                    var data = type.GetEmptyConstructor()();
                    this.logger.Debug($"Instantiated {key}");
                    return data;
                }
                else
                {
                    var data = serializer.Deserialize(type, rawData);
                    this.logger.Debug($"Loaded {key}");
                    return data;
                }
            }
        }

        private void Update(string[] keys, object[] datas)
        {
            IterTools.Zip(keys, datas).ForEach((key, data) =>
            {
                this.dataCache[key] = data;
                this.logger.Debug($"Updated {key}");
            });
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

        #if THEONE_UNITASK
        UniTask<object[]> IDataManager.LoadAsync(string[] keys, Type[] types, bool cache, IProgress<float>? progress, CancellationToken cancellationToken) => this.LoadAsync(keys, types, cache, progress, cancellationToken);

        UniTask IDataManager.SaveAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken) => this.SaveAsync(keys, progress, cancellationToken);

        UniTask IDataManager.FlushAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken) => this.FlushAsync(keys, progress, cancellationToken);

        UniTask IDataManager.SaveAllAsync(IProgress<float>? progress, CancellationToken cancellationToken) => this.SaveAsync(this.WritableKeys, progress, cancellationToken);

        UniTask IDataManager.FlushAllAsync(IProgress<float>? progress, CancellationToken cancellationToken) => this.FlushAsync(this.WritableKeys, progress, cancellationToken);

        private UniTask<object[]> LoadAsync(string[] keys, Type[] types, bool cache, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return IterTools.Zip(keys, types).SelectAsync(
                (key_type, progress, cancellationToken) => cache
                    ? this.dataCache.GetOrAddAsync(key_type.Item1, () => LoadAsync(key_type.Item1, key_type.Item2, progress, cancellationToken))
                    : LoadAsync(key_type.Item1, key_type.Item2, progress, cancellationToken),
                progress,
                cancellationToken
            ).ToArrayAsync();

            async UniTask<object> LoadAsync(string key, Type type, IProgress<float>? progress, CancellationToken cancellationToken)
            {
                var (serializer, storage) = this.GetSerializerAndStorage(type);
                var rawData = await storage.ReadAsync(key, progress, cancellationToken);
                if (rawData is null)
                {
                    var data = type.GetEmptyConstructor()();
                    this.logger.Debug($"Instantiated {key}");
                    return data;
                }
                else
                {
                    var data = await serializer.DeserializeAsync(type, rawData, cancellationToken);
                    this.logger.Debug($"Loaded {key}");
                    return data;
                }
            }
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
        IEnumerator IDataManager.LoadAsync(string[] keys, Type[] types, Action<object[]> callback, bool cache, IProgress<float>? progress) => this.LoadAsync(keys, types, callback, cache, progress);

        IEnumerator IDataManager.SaveAsync(string[] keys, Action? callback, IProgress<float>? progress) => this.SaveAsync(keys, callback, progress);

        IEnumerator IDataManager.FlushAsync(string[] keys, Action? callback, IProgress<float>? progress) => this.FlushAsync(keys, callback, progress);

        IEnumerator IDataManager.SaveAllAsync(Action? callback, IProgress<float>? progress) => this.SaveAsync(this.WritableKeys, callback, progress);

        IEnumerator IDataManager.FlushAllAsync(Action? callback, IProgress<float>? progress) => this.FlushAsync(this.WritableKeys, callback, progress);

        private IEnumerator LoadAsync(string[] keys, Type[] types, Action<object[]> callback, bool cache, IProgress<float>? progress)
        {
            return IterTools.Zip(keys, types).SelectAsync<(string, Type), object>(
                (key_type, callback, progress) => cache
                    ? this.dataCache.GetOrAddAsync(key_type.Item1, callback => LoadAsync(key_type.Item1, key_type.Item2, callback, progress), callback)
                    : LoadAsync(key_type.Item1, key_type.Item2, callback, progress),
                datas => callback(datas.ToArray()),
                progress
            );

            IEnumerator LoadAsync(string key, Type type, Action<object> callback, IProgress<float>? progress)
            {
                var (serializer, storage) = this.GetSerializerAndStorage(type);
                var rawData = default(object);
                yield return storage.ReadAsync(key, result => rawData = result, progress);
                if (rawData is null)
                {
                    var data = type.GetEmptyConstructor()();
                    this.logger.Debug($"Instantiated {key}");
                    callback(data);
                }
                else
                {
                    var data = default(object)!;
                    yield return serializer.DeserializeAsync(type, rawData, result => data = result);
                    this.logger.Debug($"Loaded {key}");
                    callback(data);
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

        private IEnumerable<string> WritableKeys => this.dataCache.WhereValue(data => this.GetSerializerAndStorage(data.GetType()).Storage is IWritableDataStorage).SelectKeys();

        private (ISerializer Serializer, IDataStorage Storage) GetSerializerAndStorage(Type type)
        {
            return this.serializerAndStorageCache.GetOrAdd(type, () =>
            {
                var serializersAndStorages = IterTools.Product(
                        this.serializers.Where(serializer => serializer.CanSerialize(type)),
                        this.storages.Where(storage => storage.CanStore(type))
                    )
                    .Where((serializer, storage) => serializer.RawDataType == storage.RawDataType)
                    .ToArray();
                if (serializersAndStorages.Length is 0) throw new InvalidOperationException($"No serializer or storage found for {type.Name}");
                return serializersAndStorages[^1];
            });
        }

        #endregion
    }
}