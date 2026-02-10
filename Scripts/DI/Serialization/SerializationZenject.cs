#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Serialization.DI
{
    using System.Globalization;
    using Zenject;
    #if UNIT_JSON
    using Newtonsoft.Json;
    using JsonSerializer = JsonSerializer;
    #endif
    #if UNIT_CSV
    using CsvHelper.Configuration;
    #endif
    #if UNIT_MEMORYPACK
    using MemoryPack;
    using MemoryPackSerializer = MemoryPackSerializer;
    #endif

    public static class SerializationZenject
    {
        public static void BindSerializers(this DiContainer container)
        {
            #if UNIT_JSON
            if (!container.HasBinding<JsonSerializerSettings>())
            {
                container.BindInstance(DefaultJsonSerializerSettings.Value);
            }
            container.BindInterfacesAndSelfTo<JsonSerializer>().AsSingle();
            #endif

            container.BindInterfacesAndSelfTo<UnityObjectSerializer>().AsSingle();

            #if UNIT_CSV
            if (!container.HasBinding<CsvConfiguration>())
            {
                container.BindInstance(new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                });
            }
            container.BindInterfacesAndSelfTo<CsvSerializer>().AsSingle();
            #endif

            #if UNIT_MEMORYPACK
            if (!container.HasBinding<MemoryPackSerializerOptions>())
            {
                container.BindInstance(MemoryPackSerializerOptions.Default);
            }
            container.BindInterfacesAndSelfTo<MemoryPackSerializer>().AsSingle();
            #endif
        }
    }
}
#endif