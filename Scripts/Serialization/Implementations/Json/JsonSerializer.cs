#if UNIT_JSON
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    public sealed class JsonSerializer : Serializer<string, IJsonData>
    {
        private readonly JsonSerializerSettings settings;

        [Preserve]
        public JsonSerializer(JsonSerializerSettings? settings = null)
        {
            this.settings = settings
                ?? new JsonSerializerSettings
                {
                    Culture                = CultureInfo.InvariantCulture,
                    TypeNameHandling       = TypeNameHandling.Auto,
                    ReferenceLoopHandling  = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                };
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