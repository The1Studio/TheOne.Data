#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.DI
{
    using System;
    using System.Collections.Generic;
    using UniT.Data.Conversion.DI;
    using UniT.Data.Serialization.DI;
    using UniT.Data.Storage.DI;
    using UniT.Logging.DI;
    using UniT.ResourceManagement.DI;
    using VContainer;

    public static class DataManagerVContainer
    {
        public static void RegisterDataManager(
            this IContainerBuilder builder,
            IEnumerable<Type>?     converterTypes   = null,
            IEnumerable<Type>?     serializerTypes  = null,
            IEnumerable<Type>?     dataStorageTypes = null
        )
        {
            if (builder.Exists(typeof(IDataManager), true)) return;
            builder.RegisterLoggerManager();
            builder.RegisterAssetsManager();
            builder.RegisterConverterManager(converterTypes);
            builder.RegisterSerializers(serializerTypes);
            builder.RegisterDataStorages(dataStorageTypes);
            builder.Register<DataManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif