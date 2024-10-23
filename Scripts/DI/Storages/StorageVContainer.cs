#if THEONE_VCONTAINER
#nullable enable
namespace TheOne.Data.Storage.DI
{
    using TheOne.ResourceManagement.DI;
    using VContainer;

    public static class StorageVContainer
    {
        public static void RegisterDataStorages(this IContainerBuilder builder)
        {
            builder.RegisterAssetsManager();

            builder.Register<AssetBinaryDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetTextDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetBlobDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<PlayerPrefsDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        public static void RegisterExternalDataStorages(this IContainerBuilder builder)
        {
            builder.RegisterExternalAssetsManager();

            builder.Register<ExternalFileVersionManager>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<ExternalBinaryDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ExternalTextDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif