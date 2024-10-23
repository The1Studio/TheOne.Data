#if THEONE_DI
#nullable enable
namespace TheOne.Data.Serialization.DI
{
    using System.Globalization;
    using TheOne.DI;
    #if THEONE_JSON
    using Newtonsoft.Json;
    using JsonSerializer = JsonSerializer;
    #endif
    #if THEONE_CSV
    using CsvHelper.Configuration;
    #endif

    public static class SerializationDI
    {
        public static void AddSerializers(this DependencyContainer container)
        {
            #if THEONE_JSON
            if (!container.Contains<JsonSerializerSettings>())
            {
                container.Add(DefaultJsonSerializerSettings.Value);
            }
            container.AddInterfacesAndSelf<JsonSerializer>();
            #endif

            container.AddInterfacesAndSelf<UnityObjectSerializer>();

            #if THEONE_CSV
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