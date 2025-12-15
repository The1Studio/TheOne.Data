#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Storage.DI
{
    using UniT.ResourceManagement.DI;
    using Zenject;

    public static class StorageZenject
    {
        public static void BindDataStorages(this DiContainer container)
        {
            container.BindAssetsManager();

            container.BindInterfacesTo<AssetBinaryDataStorage>().AsSingle();
            container.BindInterfacesTo<AssetTextDataStorage>().AsSingle();
            container.BindInterfacesTo<AssetBlobDataStorage>().AsSingle();

            container.BindInterfacesTo<PlayerPrefsDataStorage>().AsSingle();
        }

        #if UNIT_JSON
        public static void BindExternalDataStorages(this DiContainer container)
        {
            container.BindExternalAssetsManager();

            container.BindInterfacesTo<ExternalFileVersionManager>().AsSingle();

            container.BindInterfacesTo<ExternalBinaryDataStorage>().AsSingle();
            container.BindInterfacesTo<ExternalTextDataStorage>().AsSingle();
        }
        #endif
    }
}
#endif