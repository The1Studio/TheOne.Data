#if THEONE_ZENJECT
#nullable enable
namespace TheOne.Data.DI
{
    using System;
    using System.Globalization;
    using TheOne.Data.Conversion.DI;
    using TheOne.Data.Serialization.DI;
    using TheOne.Data.Storage.DI;
    using TheOne.Logging.DI;
    using Zenject;
    #if THEONE_JSON
    using Newtonsoft.Json;
    #endif
    #if THEONE_CSV
    using CsvHelper.Configuration;
    #endif

    public static class DataManagerZenject
    {
        public static void BindDataManager(this DiContainer container)
        {
            if (container.HasBinding<IDataManager>()) return;
            container.BindLoggerManager();

            #region Configs

            if (!container.HasBinding<IFormatProvider>())
            {
                container.Bind<IFormatProvider>().FromMethod(() => CultureInfo.InvariantCulture).AsSingle();
            }
            if (!container.HasBinding<SeparatorConfig>())
            {
                container.Bind<SeparatorConfig>().FromMethod(() => new SeparatorConfig()).AsSingle();
            }
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
            #endif

            #endregion

            container.BindConverterManager();
            container.BindSerializers();
            container.BindDataStorages();
            container.BindInterfacesTo<DataManager>().AsSingle();
        }
    }
}
#endif