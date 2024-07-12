#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using UniT.Data.Conversion;
    using UniT.Data.Serialization;
    using UniT.Data.Storage;
    using UniT.Logging;
    using UniT.ResourceManagement;
    using Zenject;

    public static class ZenjectBinder
    {
        public static void BindDataManager(
            this DiContainer   container,
            IEnumerable<Type>? converterTypes   = null,
            IEnumerable<Type>? serializerTypes  = null,
            IEnumerable<Type>? dataStorageTypes = null
        )
        {
            if (container.HasBinding<IDataManager>()) return;
            container.BindLoggerManager();
            container.BindAssetsManager();
            container.BindConverterManager(converterTypes);
            container.BindSerializers(serializerTypes);
            container.BindDataStorages(dataStorageTypes);
            container.BindInterfacesTo<DataManager>().AsSingle();
        }
    }
}
#endif