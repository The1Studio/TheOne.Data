#if THEONE_VCONTAINER
#nullable enable
namespace TheOne.Data.Serialization.DI
{
    using VContainer;

    public static class SerializationVContainer
    {
        public static void RegisterSerializers(this IContainerBuilder builder)
        {
            builder.Register<ObjectSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            #if THEONE_JSON
            builder.Register<JsonSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif
            #if THEONE_CSV
            builder.Register<CsvSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif
        }
    }
}
#endif