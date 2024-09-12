#if UNIT_ZENJECT
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
    using Zenject;

    public static class DataManagerZenject
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