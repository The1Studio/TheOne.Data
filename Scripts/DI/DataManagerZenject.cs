#if THEONE_ZENJECT
#nullable enable
namespace TheOne.Data.DI
{
    using TheOne.Data.Conversion.DI;
    using TheOne.Data.Serialization.DI;
    using TheOne.Data.Storage.DI;
    using TheOne.Logging.DI;
    using Zenject;

    public static class DataManagerZenject
    {
        public static void BindDataManager(this DiContainer container)
        {
            if (container.HasBinding<IDataManager>()) return;
            container.BindLoggerManager();
            container.BindConverterManager();
            container.BindSerializers();
            container.BindDataStorages();
            container.BindInterfacesTo<DataManager>().AsSingle();
        }
    }
}
#endif