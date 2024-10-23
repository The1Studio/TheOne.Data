#if THEONE_ZENJECT
#nullable enable
namespace TheOne.Data.Serialization.DI
{
    using System.Globalization;
    using Zenject;
    #if THEONE_JSON
    using Newtonsoft.Json;
    using JsonSerializer = TheOne.Data.Serialization.JsonSerializer;
    #endif
    #if THEONE_CSV
    using CsvHelper.Configuration;
    #endif

    public static class SerializationZenject
    {
        public static void BindSerializers(this DiContainer container)
        {
            container.BindInterfacesAndSelfTo<ObjectSerializer>().AsSingle();

            #if THEONE_JSON
            if (!container.HasBinding<JsonSerializerSettings>())
            {
                container.Bind<JsonSerializerSettings>().FromMethod(() => new JsonSerializerSettings
                {
                    Culture                = CultureInfo.InvariantCulture,
                    TypeNameHandling       = TypeNameHandling.Auto,
                    ReferenceLoopHandling  = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                }).AsSingle();
            }
            container.BindInterfacesAndSelfTo<JsonSerializer>().AsSingle();
            #endif

            #if THEONE_CSV
            if (!container.HasBinding<CsvConfiguration>())
            {
                container.Bind<CsvConfiguration>().FromMethod(() => new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                }).AsSingle();
            }
            container.BindInterfacesAndSelfTo<CsvSerializer>().AsSingle();
            #endif
        }
    }
}
#endif