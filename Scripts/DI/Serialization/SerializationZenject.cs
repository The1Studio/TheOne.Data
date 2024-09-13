#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Serialization.DI
{
    using Zenject;

    public static class SerializationZenject
    {
        public static void BindSerializers(this DiContainer container)
        {
            container.BindInterfacesAndSelfTo<ObjectSerializer>().AsSingle();
            #if UNIT_JSON
            container.BindInterfacesAndSelfTo<JsonSerializer>().AsSingle();
            #endif
            #if UNIT_CSV
            container.BindInterfacesAndSelfTo<CsvSerializer>().AsSingle();
            #endif
        }
    }
}
#endif