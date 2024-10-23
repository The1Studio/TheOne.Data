#nullable enable
namespace TheOne.Data.Serialization
{
    using System;
    using System.Threading;
    #if THEONE_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface ISerializer
    {
        public Type RawDataType { get; }

        public bool CanSerialize(Type type);

        public object Deserialize(Type type, object rawData);

        public object Serialize(object data);

        #if THEONE_UNITASK
        public UniTask<object> DeserializeAsync(Type type, object rawData, CancellationToken cancellationToken = default);

        public UniTask<object> SerializeAsync(object data, CancellationToken cancellationToken = default);
        #else
        public IEnumerator DeserializeAsync(Type type, object rawData, Action<object> callback);

        public IEnumerator SerializeAsync(object data, Action<object> callback);
        #endif
    }
}