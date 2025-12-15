#if UNIT_DI
#nullable enable
namespace UniT.Data.Storage.DI
{
    using UniT.DI;
    using UniT.ResourceManagement.DI;

    public static class StorageDI
    {
        public static void AddDataStorages(this DependencyContainer container)
        {
            container.AddAssetsManager();

            container.AddInterfaces<AssetBinaryDataStorage>();
            container.AddInterfaces<AssetTextDataStorage>();
            container.AddInterfaces<AssetBlobDataStorage>();

            container.AddInterfaces<PlayerPrefsDataStorage>();
        }

        #if UNIT_JSON
        public static void AddExternalDataStorages(this DependencyContainer container)
        {
            container.AddExternalAssetsManager();

            container.AddInterfaces<ExternalFileVersionManager>();

            container.AddInterfaces<ExternalBinaryDataStorage>();
            container.AddInterfaces<ExternalTextDataStorage>();
        }
        #endif
    }
}
#endif