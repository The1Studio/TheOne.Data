#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Serialization.DI
{
    using System.Globalization;
    using Zenject;
    #if UNIT_JSON
    using Newtonsoft.Json;
    using JsonSerializer = UniT.Data.Serialization.JsonSerializer;
    #endif
    #if UNIT_CSV
    using CsvHelper.Configuration;
    #endif

    public static class SerializationZenject
    {
        public static void BindSerializers(this DiContainer container)
        {
            #if UNIT_JSON
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

            container.BindInterfacesAndSelfTo<UnityObjectSerializer>().AsSingle();

            #if UNIT_CSV
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