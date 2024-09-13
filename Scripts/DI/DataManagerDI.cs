#if UNIT_DI
#nullable enable
namespace UniT.Data.DI
{
    using System;
    using System.Globalization;
    using CsvHelper.Configuration;
    using Newtonsoft.Json;
    using UniT.Data.Conversion.DI;
    using UniT.Data.Serialization.DI;
    using UniT.Data.Storage.DI;
    using UniT.DI;
    using UniT.Logging.DI;
    using UniT.ResourceManagement.DI;

    public static class DataManagerDI
    {
        public static void AddDataManager(this DependencyContainer container)
        {
            if (container.Contains<IDataManager>()) return;
            container.AddLoggerManager();
            container.AddAssetsManager();

            #region Configs

            if (!container.Contains<SeparatorConfig>())
            {
                container.Add(new SeparatorConfig());
            }
            if (!container.Contains<IFormatProvider>())
            {
                container.Add((IFormatProvider)CultureInfo.InvariantCulture);
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
            container.AddDataStorages();
            container.AddInterfaces<DataManager>();
        }
    }
}
#endif