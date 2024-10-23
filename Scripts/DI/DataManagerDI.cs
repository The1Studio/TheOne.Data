#if THEONE_DI
#nullable enable
namespace TheOne.Data.DI
{
    using TheOne.Data.Conversion.DI;
    using TheOne.Data.Serialization.DI;
    using TheOne.Data.Storage.DI;
    using TheOne.DI;
    using TheOne.Logging.DI;

    public static class DataManagerDI
    {
        public static void AddDataManager(this DependencyContainer container)
        {
            if (container.Contains<IDataManager>()) return;
            container.AddLoggerManager();
            container.AddConverterManager();
            container.AddSerializers();
            container.AddDataStorages();
            container.AddInterfaces<DataManager>();
        }
    }
}
#endif