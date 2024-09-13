#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Storage.DI
{
    using Zenject;

    public static class StorageZenject
    {
        public static void BindDataStorages(this DiContainer container)
        {
            container.BindInterfacesAndSelfTo<AssetBinaryDataStorage>().AsSingle();
            container.BindInterfacesAndSelfTo<AssetTextDataStorage>().AsSingle();
            container.BindInterfacesAndSelfTo<AssetBlobDataStorage>().AsSingle();
            container.BindInterfacesAndSelfTo<PlayerPrefsDataStorage>().AsSingle();
        }
    }
}
#endif