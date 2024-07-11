#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using UnityEngine.Scripting;

    public sealed class ObjectSerializer : Serializer<object, IData>
    {
        [Preserve]
        public ObjectSerializer()
        {
        }

        protected override IData Deserialize(Type type, object rawData) => (IData)rawData;

        protected override object Serialize(IData data) => data;
    }
}