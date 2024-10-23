#if THEONE_VCONTAINER
#nullable enable
namespace TheOne.Data.Serialization.DI
{
    using System.Globalization;
    using VContainer;
    #if THEONE_JSON
    using Newtonsoft.Json;
    using JsonSerializer = TheOne.Data.Serialization.JsonSerializer;
    #endif
    #if THEONE_CSV
    using CsvHelper.Configuration;
    #endif

    public static class SerializationVContainer
    {
        public static void RegisterSerializers(this IContainerBuilder builder)
        {
            #if THEONE_JSON
            if (!builder.Exists(typeof(JsonSerializerSettings)))
            {
                builder.Register(_ => new JsonSerializerSettings
                {
                    Culture                = CultureInfo.InvariantCulture,
                    TypeNameHandling       = TypeNameHandling.Auto,
                    ReferenceLoopHandling  = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                }, Lifetime.Singleton);
            }
            builder.Register<JsonSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #endif

            builder.Register<UnityObjectSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            #if THEONE_CSV
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