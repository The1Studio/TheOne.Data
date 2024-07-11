#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Threading;
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface ISerializer
    {
        public Type RawDataType { get; }

        public bool CanSerialize(Type type);

        public IData Deserialize(Type type, object rawData);

        public object Serialize(IData data);

        #if UNIT_UNITASK
        public UniTask<IData> DeserializeAsync(Type type, object rawData, CancellationToken cancellationToken = default);

        public UniTask<object> SerializeAsync(IData data, CancellationToken cancellationToken = default);
        #else
        public IEnumerator DeserializeAsync(Type type, object rawData, Action<IData> callback);

        public IEnumerator SerializeAsync(IData data, Action<object> callback);
        #endif
    }
}