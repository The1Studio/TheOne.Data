#if UNIT_DI
#nullable enable
namespace UniT.Data.DI
{
    using System;
    using System.Collections.Generic;
    using UniT.Data.Conversion.DI;
    using UniT.Data.Serialization.DI;
    using UniT.Data.Storage.DI;
    using UniT.DI;
    using UniT.Logging.DI;
    using UniT.ResourceManagement.DI;

    public static class DataManagerDI
    {
        public static void AddDataManager(
            this DependencyContainer container,
            IEnumerable<Type>?       converterTypes   = null,
            IEnumerable<Type>?       serializerTypes  = null,
            IEnumerable<Type>?       dataStorageTypes = null
        )
        {
            if (container.Contains<IDataManager>()) return;
            container.AddLoggerManager();
            container.AddAssetsManager();
            container.AddConverterManager(converterTypes);
            container.AddSerializers(serializerTypes);
            container.AddDataStorages(dataStorageTypes);
            container.AddInterfaces<DataManager>();
        }
    }
}
#endif