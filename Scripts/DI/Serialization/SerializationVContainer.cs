#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.Serialization.DI
{
    using VContainer;

    public static class SerializationVContainer
    {
        public static void RegisterSerializers(this IContainerBuilder builder)
        {
            builder.Register<ObjectSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            #if UNIT_JSON
            builder.Register<JsonSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif
            #if UNIT_CSV
            builder.Register<CsvSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif
        }
    }
}
#endif