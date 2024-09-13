#if UNIT_DI
#nullable enable
namespace UniT.Data.Serialization.DI
{
    using UniT.DI;

    public static class SerializationDI
    {
        public static void AddSerializers(this DependencyContainer container)
        {
            container.AddInterfacesAndSelf<ObjectSerializer>();
            #if UNIT_JSON
            container.AddInterfacesAndSelf<JsonSerializer>();
            #endif
            #if UNIT_CSV
            container.AddInterfacesAndSelf<CsvSerializer>();
            #endif
        }
    }
}
#endif