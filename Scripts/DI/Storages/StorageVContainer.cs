#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.Storage.DI
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using VContainer;

    public static class StorageVContainer
    {
        public static void RegisterDataStorages(this IContainerBuilder builder, IEnumerable<Type>? dataStorageTypes = null)
        {
            builder.Register<AssetBinaryDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetTextDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetBlobDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlayerPrefsDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();

            dataStorageTypes?.ForEach(type =>
            {
                if (!typeof(IDataStorage).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IDataStorage)}");
                builder.Register(type, Lifetime.Singleton).AsImplementedInterfaces();
            });
        }
    }
}
#endif