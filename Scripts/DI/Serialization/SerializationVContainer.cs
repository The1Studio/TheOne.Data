#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.Serialization.DI
{
    using VContainer;

    public static class SerializationVContainer
    {
        public static void RegisterSerializers(this IContainerBuilder builder)
        {
            builder.Register<ObjectSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #if UNIT_JSON
            builder.Register<JsonSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #endif
            #if UNIT_CSV
            builder.Register<CsvSerializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            #endif
        }
    }
}
#endif