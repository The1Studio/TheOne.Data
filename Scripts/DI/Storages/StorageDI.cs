#if UNIT_DI
#nullable enable
namespace UniT.Data.Storage.DI
{
    using UniT.DI;
    using UniT.ResourceManagement.DI;

    public static class StorageDI
    {
        public static void AddLocalDataStorages(this DependencyContainer container)
        {
            container.AddAssetsManager();

            container.AddInterfaces<AssetBinaryDataStorage>();
            container.AddInterfaces<AssetTextDataStorage>();
            container.AddInterfaces<AssetBlobDataStorage>();

            container.AddInterfaces<PlayerPrefsDataStorage>();
        }

        public static void AddRemoteDataStorages(this DependencyContainer container)
        {
            container.AddExternalAssetsManager();

            container.AddInterfaces<RemoteFileVersionManager>();

            container.AddInterfaces<RemoteBinaryDataStorage>();
            container.AddInterfaces<RemoteTextDataStorage>();
        }
    }
}
#endif