#if THEONE_DI
#nullable enable
namespace TheOne.Data.Serialization.DI
{
    using TheOne.DI;

    public static class SerializationDI
    {
        public static void AddSerializers(this DependencyContainer container)
        {
            container.AddInterfacesAndSelf<ObjectSerializer>();
            #if THEONE_JSON
            container.AddInterfacesAndSelf<JsonSerializer>();
            #endif
            #if THEONE_CSV
            container.AddInterfacesAndSelf<CsvSerializer>();
            #endif
        }
    }
}
#endif