#if UNIT_JSON
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    public sealed class JsonSerializer : Serializer<string, IJsonData>
    {
        private readonly JsonSerializerSettings settings;

        [Preserve]
        public JsonSerializer(JsonSerializerSettings settings)
        {
            this.settings = settings;
        }

        public override IJsonData Deserialize(Type type, string rawData)
        {
            return (IJsonData)JsonConvert.DeserializeObject(rawData, type, this.settings)!;
        }

        public override string Serialize(IJsonData data)
        {
            return JsonConvert.SerializeObject(data, this.settings);
        }
    }
}
#endif