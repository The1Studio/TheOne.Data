#if THEONE_JSON
#nullable enable
namespace TheOne.Data.Conversion
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    public sealed class JsonConverter : Converter<object>
    {
        private readonly JsonSerializerSettings settings;

        [Preserve]
        public JsonConverter(JsonSerializerSettings settings)
        {
            this.settings = settings;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return JsonConvert.DeserializeObject(str, type, this.settings)!;
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, this.settings);
        }
    }
}
#endif