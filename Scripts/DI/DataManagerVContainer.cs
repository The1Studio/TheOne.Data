#if THEONE_VCONTAINER
#nullable enable
namespace TheOne.Data.DI
{
    using TheOne.Data.Conversion.DI;
    using TheOne.Data.Serialization.DI;
    using TheOne.Data.Storage.DI;
    using TheOne.Logging.DI;
    using VContainer;

    public static class DataManagerVContainer
    {
        public static void RegisterDataManager(this IContainerBuilder builder)
        {
            if (builder.Exists(typeof(IDataManager), true)) return;
            builder.RegisterLoggerManager();
            builder.RegisterConverterManager();
            builder.RegisterSerializers();
            builder.RegisterDataStorages();
            builder.Register<DataManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif