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

            container.AddInterfacesAndSelf<AssetBinaryDataStorage>();
            container.AddInterfacesAndSelf<AssetTextDataStorage>();
            container.AddInterfacesAndSelf<AssetBlobDataStorage>();

            container.AddInterfacesAndSelf<PlayerPrefsDataStorage>();
        }

        public static void AddExternalDataStorages(this DependencyContainer container)
        {
            container.AddInterfaces<ExternalFileVersionManager>();

            container.AddInterfacesAndSelf<ExternalBinaryDataStorage>();
            container.AddInterfacesAndSelf<ExternalTextDataStorage>();
        }
    }
}
#endif