#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Storage.DI
{
    using UniT.ResourceManagement.DI;
    using Zenject;

    public static class StorageZenject
    {
        public static void BindLocalDataStorages(this DiContainer container)
        {
            container.BindAssetsManager();

            container.BindInterfacesTo<AssetBinaryDataStorage>().AsSingle();
            container.BindInterfacesTo<AssetTextDataStorage>().AsSingle();
            container.BindInterfacesTo<AssetBlobDataStorage>().AsSingle();

            container.BindInterfacesTo<PlayerPrefsDataStorage>().AsSingle();
        }

        public static void BindRemoteDataStorages(this DiContainer container)
        {
            container.BindExternalAssetsManager();

            container.BindInterfacesTo<RemoteFileVersionManager>().AsSingle();

            container.BindInterfacesTo<RemoteBinaryDataStorage>().AsSingle();
            container.BindInterfacesTo<RemoteTextDataStorage>().AsSingle();
        }
    }
}
#endif