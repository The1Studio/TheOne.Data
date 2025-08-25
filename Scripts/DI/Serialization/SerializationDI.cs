#if UNIT_DI
#nullable enable
namespace UniT.Data.Serialization.DI
{
    using System.Globalization;
    using UniT.DI;
    #if UNIT_JSON
    using Newtonsoft.Json;
    using JsonSerializer = UniT.Data.Serialization.JsonSerializer;
    #endif
    #if UNIT_CSV
    using CsvHelper.Configuration;
    #endif

    public static class SerializationDI
    {
        public static void AddSerializers(this DependencyContainer container)
        {
            #if UNIT_JSON
            if (!container.Contains<JsonSerializerSettings>())
            {
                container.Add(new JsonSerializerSettings
                {
                    Culture                = CultureInfo.InvariantCulture,
                    TypeNameHandling       = TypeNameHandling.Auto,
                    ReferenceLoopHandling  = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                });
            }
            container.AddInterfacesAndSelf<JsonSerializer>();
            #endif

            container.AddInterfacesAndSelf<UnityObjectSerializer>();

            #if UNIT_CSV
            if (!container.Contains<CsvConfiguration>())
            {
                container.Add(new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                });
            }
            container.AddInterfacesAndSelf<CsvSerializer>();
            #endif
        }
    }
}
#endif