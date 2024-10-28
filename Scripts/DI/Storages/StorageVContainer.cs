#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.Storage.DI
{
    using UniT.ResourceManagement.DI;
    using VContainer;

    public static class StorageVContainer
    {
        public static void RegisterLocalDataStorages(this IContainerBuilder builder)
        {
            builder.RegisterAssetsManager();

            builder.Register<AssetBinaryDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetTextDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetBlobDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<PlayerPrefsDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        public static void RegisterRemoteDataStorages(this IContainerBuilder builder)
        {
            builder.RegisterExternalAssetsManager();

            builder.Register<RemoteFileVersionManager>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<RemoteBinaryDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<RemoteTextDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif