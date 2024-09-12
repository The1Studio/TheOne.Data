#if UNIT_DI
#nullable enable
namespace UniT.Data.Storage.DI
{
    using System;
    using System.Collections.Generic;
    using UniT.DI;
    using UniT.Extensions;

    public static class StorageDI
    {
        public static void AddDataStorages(this DependencyContainer container, IEnumerable<Type>? dataStorageTypes = null)
        {
            container.AddInterfacesAndSelf<AssetBinaryDataStorage>();
            container.AddInterfacesAndSelf<AssetTextDataStorage>();
            container.AddInterfacesAndSelf<AssetBlobDataStorage>();
            container.AddInterfacesAndSelf<PlayerPrefsDataStorage>();

            dataStorageTypes?.ForEach(type =>
            {
                if (!typeof(IDataStorage).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IDataStorage)}");
                container.AddInterfacesAndSelf(type);
            });
        }
    }
}
#endif