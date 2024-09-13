#if UNIT_DI
#nullable enable
namespace UniT.Data.Storage.DI
{
    using UniT.DI;

    public static class StorageDI
    {
        public static void AddDataStorages(this DependencyContainer container)
        {
            container.AddInterfacesAndSelf<AssetBinaryDataStorage>();
            container.AddInterfacesAndSelf<AssetTextDataStorage>();
            container.AddInterfacesAndSelf<AssetBlobDataStorage>();
            container.AddInterfacesAndSelf<PlayerPrefsDataStorage>();
        }
    }
}
#endif