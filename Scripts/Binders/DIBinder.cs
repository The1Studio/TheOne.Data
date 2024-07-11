#if UNIT_DI
#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using UniT.Data.Conversion;
    using UniT.Data.Serialization;
    using UniT.Data.Storage;
    using UniT.DI;
    using UniT.Logging;
    using UniT.ResourceManagement;

    public static class DIBinder
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
            container.AddResourceManagers();
            container.AddConverterManager(converterTypes);
            container.AddSerializers(serializerTypes);
            container.AddDataStorages(dataStorageTypes);
            container.AddInterfaces<DataManager>();
        }
    }
}
#endif