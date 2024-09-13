#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.Storage.DI
{
    using VContainer;

    public static class StorageVContainer
    {
        public static void RegisterDataStorages(this IContainerBuilder builder)
        {
            builder.Register<AssetBinaryDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetTextDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetBlobDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlayerPrefsDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif