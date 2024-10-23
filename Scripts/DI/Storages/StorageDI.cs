#if THEONE_DI
#nullable enable
namespace TheOne.Data.Storage.DI
{
    using TheOne.DI;
    using TheOne.ResourceManagement.DI;

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

        public static void AddExternalDataStorages(this DependencyContainer container)
        {
            container.AddExternalAssetsManager();

            container.AddInterfaces<ExternalFileVersionManager>();

            container.AddInterfaces<ExternalBinaryDataStorage>();
            container.AddInterfaces<ExternalTextDataStorage>();
        }
    }
}
#endif