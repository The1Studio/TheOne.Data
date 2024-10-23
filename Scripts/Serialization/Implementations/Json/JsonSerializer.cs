#if THEONE_JSON
#nullable enable
namespace TheOne.Data.Serialization
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

        protected override IJsonData Deserialize(Type type, string rawData)
        {
            return (IJsonData)JsonConvert.DeserializeObject(rawData, type, this.settings)!;
        }

        protected override string Serialize(IJsonData data)
        {
            return JsonConvert.SerializeObject(data, this.settings);
        }
    }
}
#endif