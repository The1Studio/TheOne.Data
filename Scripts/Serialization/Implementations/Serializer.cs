#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Threading;
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    using UniT.Extensions;
    #endif

    public abstract class Serializer<TRawData, TData> : ISerializer where TRawData : notnull where TData : notnull
    {
        Type ISerializer.RawDataType => typeof(TRawData);

        bool ISerializer.CanSerialize(Type type) => typeof(TData).IsAssignableFrom(type);

        object ISerializer.Deserialize(Type type, object rawData) => this.Deserialize(type, (TRawData)rawData);

        object ISerializer.Serialize(object data) => this.Serialize((TData)data);

        public abstract TData Deserialize(Type type, TRawData rawData);

        public abstract TRawData Serialize(TData data);

        public T Deserialize<T>(TRawData rawData) where T : TData => (T)this.Deserialize(typeof(T), rawData);

        #if UNIT_UNITASK
        UniTask<object> ISerializer.DeserializeAsync(Type type, object rawData, CancellationToken cancellationToken) => this.DeserializeAsync(type, (TRawData)rawData, cancellationToken).ContinueWith(data => (object)data);

        UniTask<object> ISerializer.SerializeAsync(object data, CancellationToken cancellationToken) => this.SerializeAsync((TData)data, cancellationToken).ContinueWith(rawData => (object)rawData);

        public virtual UniTask<TData> DeserializeAsync(Type type, TRawData rawData, CancellationToken cancellationToken)
        {
            #if !UNITY_WEBGL
            return UniTask.RunOnThreadPool(() => this.Deserialize(type, rawData), cancellationToken: cancellationToken);
            #else
            return UniTask.FromResult(this.Deserialize(type, rawData));
            #endif
        }

        public virtual UniTask<TRawData> SerializeAsync(TData data, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(this.Serialize(data));
        }
        #else
        IEnumerator ISerializer.DeserializeAsync(Type type, object rawData, Action<object> callback) => this.DeserializeAsync(type, (TRawData)rawData, data => callback(data));

        IEnumerator ISerializer.SerializeAsync(object data, Action<object> callback) => this.SerializeAsync((TData)data, rawData => callback(rawData));

        public virtual IEnumerator DeserializeAsync(Type type, TRawData rawData, Action<TData> callback) => CoroutineRunner.Run(() => this.Deserialize(type, rawData), callback);

        public virtual IEnumerator SerializeAsync(TData data, Action<TRawData> callback)
        {
            callback(this.Serialize(data));
            yield break;
        }
        #endif
    }
}