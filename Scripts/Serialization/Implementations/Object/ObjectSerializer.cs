#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using UnityEngine.Scripting;

    public sealed class ObjectSerializer : Serializer<object, object>
    {
        [Preserve]
        public ObjectSerializer()
        {
        }

        public override object Deserialize(Type type, object rawData) => rawData;

        public override object Serialize(object data) => data;
    }
}