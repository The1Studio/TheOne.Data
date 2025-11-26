#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.Serialization.DI
{
    using System.Globalization;
    using VContainer;
    #if UNIT_JSON
    using Newtonsoft.Json;
    using JsonSerializer = JsonSerializer;
    #endif
    #if UNIT_CSV
    using CsvHelper.Configuration;
    #endif

    public static class SerializationVContainer
    {
        public static void RegisterSerializers(this IContainerBuilder builder)
        {
            #if UNIT_JSON
            if (!builder.Exists(typeof(JsonSerializerSettings)))
            {
                builder.Register(_ => DefaultJsonSerializerSettings.Value, Lifetime.Singleton);
            }
            builder.Register<JsonSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #endif

            builder.Register<UnityObjectSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            #if UNIT_CSV
            if (!builder.Exists(typeof(CsvConfiguration)))
            {
                builder.Register(_ => new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                }, Lifetime.Singleton);
            }
            builder.Register<CsvSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #endif
        }
    }
}
#endif