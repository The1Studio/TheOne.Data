#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.DI
{
    using System;
    using System.Globalization;
    using UniT.Data.Conversion.DI;
    using UniT.Data.Serialization.DI;
    using UniT.Data.Storage.DI;
    using UniT.Logging.DI;
    using Zenject;
    #if UNIT_JSON
    using Newtonsoft.Json;
    #endif
    #if UNIT_CSV
    using CsvHelper.Configuration;
    #endif

    public static class DataManagerZenject
    {
        public static void BindDataManager(this DiContainer container)
        {
            if (container.HasBinding<IDataManager>()) return;
            container.BindLoggerManager();
            container.BindDataConfigs();
            container.BindConverterManager();
            container.BindSerializers();
            container.BindDataStorages();
            container.BindInterfacesTo<DataManager>().AsSingle();
        }

        public static void BindDataConfigs(this DiContainer container)
        {
            if (!container.HasBinding<IFormatProvider>())
            {
                container.Bind<IFormatProvider>().FromMethod(() => CultureInfo.InvariantCulture).AsSingle();
            }
            if (!container.HasBinding<SeparatorConfig>())
            {
                container.Bind<SeparatorConfig>().FromMethod(() => new SeparatorConfig()).AsSingle();
            }
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
            #endif
            #if UNIT_CSV
            if (!container.HasBinding<CsvConfiguration>())
            {
                container.Bind<CsvConfiguration>().FromMethod(() => new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                }).AsSingle();
            }
            #endif
        }
    }
}
#endif