#if THEONE_VCONTAINER
#nullable enable
namespace TheOne.Data.Serialization.DI
{
    using System.Globalization;
    using VContainer;
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

    public static class SerializationVContainer
    {
        public static void RegisterSerializers(this IContainerBuilder builder)
        {
            #if THEONE_JSON
            if (!builder.Exists(typeof(JsonSerializerSettings)))
            {
                builder.RegisterInstance(DefaultJsonSerializerSettings.Value);
            }
            builder.Register<JsonSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #endif

            builder.Register<UnityObjectSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            #if THEONE_CSV
            if (!builder.Exists(typeof(CsvConfiguration)))
            {
                builder.RegisterInstance(new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                });
            }
            builder.Register<CsvSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #endif

            #if THEONE_MEMORYPACK
            if (!builder.Exists(typeof(MemoryPackSerializerOptions)))
            {
                builder.RegisterInstance(MemoryPackSerializerOptions.Default);
            }
            builder.Register<MemoryPackSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #endif

            #if THEONE_MESSAGEPACK
            if (!builder.Exists(typeof(MessagePackSerializerOptions)))
            {
                builder.RegisterInstance(MessagePackSerializerOptions.Standard);
            }
            builder.Register<MessagePackSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #endif
        }
    }
}
#endif