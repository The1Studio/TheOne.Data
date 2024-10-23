#if THEONE_ZENJECT
#nullable enable
namespace TheOne.Data.Storage.DI
{
    using TheOne.ResourceManagement.DI;
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

        public static void BindExternalDataStorages(this DiContainer container)
        {
            container.BindExternalAssetsManager();

            container.BindInterfacesTo<ExternalFileVersionManager>().AsSingle();

            container.BindInterfacesTo<ExternalBinaryDataStorage>().AsSingle();
            container.BindInterfacesTo<ExternalTextDataStorage>().AsSingle();
        }
    }
}
#endif