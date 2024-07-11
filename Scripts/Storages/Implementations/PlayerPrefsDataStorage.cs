#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class PlayerPrefsDataStorage : IReadableDataStorage, IWritableDataStorage
    {
        [Preserve]
        public PlayerPrefsDataStorage()
        {
        }

        public Type RawDataType => typeof(string);

        bool IDataStorage.CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type) && typeof(IWritableData).IsAssignableFrom(type);

        object? IReadableDataStorage.Read(string key) => Read(key);

        void IWritableDataStorage.Write(string key, object value) => Write(key, value);

        void IWritableDataStorage.Flush() => Flush();

        #if UNIT_UNITASK
        UniTask<object?> IReadableDataStorage.ReadAsync(string key,  IProgress<float>? progress, CancellationToken cancellationToken)
        {
            var rawData = Read(key);
            progress?.Report(1);
            return UniTask.FromResult(rawData);
        }

        UniTask IWritableDataStorage.WriteAsync(string key, object value,  IProgress<float>? progress, CancellationToken cancellationToken)
        {
            Write(key, value);
            progress?.Report(1);
            return UniTask.CompletedTask;
        }

        UniTask IWritableDataStorage.FlushAsync(IProgress<float>? progress, CancellationToken cancellationToken)
        {
            Flush();
            progress?.Report(1);
            return UniTask.CompletedTask;
        }
        #else
        IEnumerator IReadableDataStorage.ReadAsync(string key, Action<object?> callback, IProgress<float>? progress)
        {
            var rawData = Read(key);
            progress?.Report(1);
            callback(rawData);
            yield break;
        }

        IEnumerator IWritableDataStorage.WriteAsync(string key, object value, Action? callback, IProgress<float>? progress)
        {
            Write(key, value);
            progress?.Report(1);
            callback?.Invoke();
            yield break;
        }

        IEnumerator IWritableDataStorage.FlushAsync(Action? callback, IProgress<float>? progress)
        {
            Flush();
            progress?.Report(1);
            callback?.Invoke();
            yield break;
        }
        #endif

        private static object? Read(string key) => PlayerPrefs.GetString(key).NullIfWhitespace();

        private static void Write(string key, object value) => PlayerPrefs.SetString(key, (string)value);

        private static void Flush() => PlayerPrefs.Save();
    }
}