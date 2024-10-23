#if THEONE_DI
#nullable enable
namespace TheOne.Data.DI
{
    using System;
    using System.Globalization;
    using TheOne.Data.Conversion.DI;
    using TheOne.Data.Serialization.DI;
    using TheOne.Data.Storage.DI;
    using TheOne.DI;
    using TheOne.Logging.DI;
    #if THEONE_JSON
    using Newtonsoft.Json;
    #endif
    #if THEONE_CSV
    using CsvHelper.Configuration;
    #endif

    public static class DataManagerDI
    {
        public static void AddDataManager(this DependencyContainer container)
        {
            if (container.Contains<IDataManager>()) return;
            container.AddLoggerManager();
            container.AddDataConfigs();
            container.AddConverterManager();
            container.AddSerializers();
            container.AddDataStorages();
            container.AddInterfaces<DataManager>();
        }

        public static void AddDataConfigs(this DependencyContainer container)
        {
            if (!container.Contains<IFormatProvider>())
            {
                container.Add((IFormatProvider)CultureInfo.InvariantCulture);
            }
            if (!container.Contains<SeparatorConfig>())
            {
                container.Add(new SeparatorConfig());
            }
            #if THEONE_JSON
            if (!container.Contains<JsonSerializerSettings>())
            {
                container.Add(new JsonSerializerSettings
                {
                    Culture                = CultureInfo.InvariantCulture,
                    TypeNameHandling       = TypeNameHandling.Auto,
                    ReferenceLoopHandling  = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                });
            }
            #endif
            #if THEONE_CSV
            if (!container.Contains<CsvConfiguration>())
            {
                container.Add(new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                });
            }
            #endif
        }
    }
}
#endif