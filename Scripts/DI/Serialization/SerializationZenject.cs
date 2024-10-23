#if THEONE_ZENJECT
#nullable enable
namespace TheOne.Data.Serialization.DI
{
    using System.Globalization;
    using Zenject;
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

    public static class SerializationZenject
    {
        public static void BindSerializers(this DiContainer container)
        {
            #if THEONE_JSON
            if (!container.HasBinding<JsonSerializerSettings>())
            {
                container.BindInstance(DefaultJsonSerializerSettings.Value);
            }
            container.BindInterfacesAndSelfTo<JsonSerializer>().AsSingle();
            #endif

            container.BindInterfacesAndSelfTo<UnityObjectSerializer>().AsSingle();

            #if THEONE_CSV
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

            #if THEONE_MEMORYPACK
            if (!container.HasBinding<MemoryPackSerializerOptions>())
            {
                container.BindInstance(MemoryPackSerializerOptions.Default);
            }
            container.BindInterfacesAndSelfTo<MemoryPackSerializer>().AsSingle();
            #endif

            #if THEONE_MESSAGEPACK
            if (!container.HasBinding<MessagePackSerializerOptions>())
            {
                container.BindInstance(MessagePackSerializerOptions.Standard);
            }
            container.BindInterfacesAndSelfTo<MessagePackSerializer>().AsSingle();
            #endif
        }
    }
}
#endif