#if THEONE_ZENJECT
#nullable enable
namespace TheOne.Data.Serialization.DI
{
    using Zenject;

    public static class SerializationZenject
    {
        public static void BindSerializers(this DiContainer container)
        {
            container.BindInterfacesAndSelfTo<ObjectSerializer>().AsSingle();
            #if THEONE_JSON
            container.BindInterfacesAndSelfTo<JsonSerializer>().AsSingle();
            #endif
            #if THEONE_CSV
            container.BindInterfacesAndSelfTo<CsvSerializer>().AsSingle();
            #endif
        }
    }
}
#endif