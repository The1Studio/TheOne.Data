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
    #if THEONE_MEMORYPACK
    using MemoryPack;
    using MemoryPackSerializer = MemoryPackSerializer;
    #endif
    #if THEONE_MESSAGEPACK
    using MessagePack;
    using MessagePackSerializer = MessagePackSerializer;
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

            #if THEONE_MEMORYPACK
            if (!container.Contains<MemoryPackSerializerOptions>())
            {
                container.Add(MemoryPackSerializerOptions.Default);
            }
            container.AddInterfacesAndSelf<MemoryPackSerializer>();
            #endif

            #if THEONE_MESSAGEPACK
            if (!container.Contains<MessagePackSerializerOptions>())
            {
                container.Add(MessagePackSerializerOptions.Standard);
            }
            container.AddInterfacesAndSelf<MessagePackSerializer>();
            #endif
        }
    }
}
#endif