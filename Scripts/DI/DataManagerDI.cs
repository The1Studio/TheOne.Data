#if UNIT_DI
#nullable enable
namespace UniT.Data.DI
{
    using System;
    using System.Globalization;
    using UniT.Data.Conversion.DI;
    using UniT.Data.Serialization.DI;
    using UniT.Data.Storage.DI;
    using UniT.DI;
    using UniT.Logging.DI;
    #if UNIT_JSON
    using Newtonsoft.Json;
    #endif
    #if UNIT_CSV
    using CsvHelper.Configuration;
    #endif

    public static class DataManagerDI
    {
        public static void AddDataManager(this DependencyContainer container)
        {
            if (container.Contains<IDataManager>()) return;
            container.AddLoggerManager();

            #region Configs

            if (!container.Contains<IFormatProvider>())
            {
                container.Add((IFormatProvider)CultureInfo.InvariantCulture);
            }
            if (!container.Contains<SeparatorConfig>())
            {
                container.Add(new SeparatorConfig());
            }
            #if UNIT_JSON
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
            #if UNIT_CSV
            if (!container.Contains<CsvConfiguration>())
            {
                container.Add(new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                });
            }
            #endif

            #endregion

            container.AddConverterManager();
            container.AddSerializers();
            container.AddLocalDataStorages();
            container.AddInterfaces<DataManager>();
        }
    }
}
#endif