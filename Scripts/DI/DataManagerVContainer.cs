#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.DI
{
    using System;
    using System.Globalization;
    using UniT.Data.Conversion.DI;
    using UniT.Data.Serialization.DI;
    using UniT.Data.Storage.DI;
    using UniT.Logging.DI;
    using VContainer;
    #if UNIT_JSON
    using Newtonsoft.Json;
    #endif
    #if UNIT_CSV
    using CsvHelper.Configuration;
    #endif

    public static class DataManagerVContainer
    {
        public static void RegisterDataManager(this IContainerBuilder builder)
        {
            if (builder.Exists(typeof(IDataManager), true)) return;
            builder.RegisterLoggerManager();

            #region Configs

            if (!builder.Exists(typeof(IFormatProvider), true))
            {
                builder.Register(_ => (IFormatProvider)CultureInfo.InvariantCulture, Lifetime.Singleton);
            }
            if (!builder.Exists(typeof(SeparatorConfig)))
            {
                builder.Register(_ => new SeparatorConfig(), Lifetime.Singleton);
            }
            #if UNIT_JSON
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
            #endif
            #if UNIT_CSV
            if (!builder.Exists(typeof(CsvConfiguration)))
            {
                builder.Register(_ => new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                }, Lifetime.Singleton);
            }
            #endif

            #endregion

            builder.RegisterConverterManager();
            builder.RegisterSerializers();
            builder.RegisterDataStorages();
            builder.Register<DataManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif