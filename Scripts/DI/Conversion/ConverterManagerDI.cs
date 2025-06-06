﻿#if THEONE_DI
#nullable enable
namespace TheOne.Data.Conversion.DI
{
    using TheOne.DI;

    public static class ConverterManagerDI
    {
        public static void AddConverterManager(this DependencyContainer container)
        {
            if (container.Contains<IConverterManager>()) return;

            #region Converters

            #if THEONE_JSON
            container.AddInterfaces<JsonConverter>();
            #endif

            #region Numbers

            container.AddInterfaces<ByteConverter>();
            container.AddInterfaces<Int16Converter>();
            container.AddInterfaces<Int32Converter>();
            container.AddInterfaces<Int64Converter>();
            container.AddInterfaces<UInt16Converter>();
            container.AddInterfaces<UInt32Converter>();
            container.AddInterfaces<UInt64Converter>();
            container.AddInterfaces<SingleConverter>();
            container.AddInterfaces<DoubleConverter>();
            container.AddInterfaces<DecimalConverter>();

            #endregion

            #region Time

            container.AddInterfaces<DateTimeConverter>();
            container.AddInterfaces<DateTimeOffsetConverter>();
            container.AddInterfaces<TimeSpanConverter>();

            #endregion

            #region Others

            container.AddInterfaces<BooleanConverter>();
            container.AddInterfaces<CharConverter>();
            container.AddInterfaces<StringConverter>();
            container.AddInterfaces<EnumConverter>();
            container.AddInterfaces<NullableConverter>();
            container.AddInterfaces<UriConverter>();
            container.AddInterfaces<GuidConverter>();

            #endregion

            #region Tuples

            container.AddInterfaces<TupleConverter>();
            container.AddInterfaces<Vector2Converter>();    // Depend on TupleConverter
            container.AddInterfaces<Vector3Converter>();    // Depend on TupleConverter
            container.AddInterfaces<Vector4Converter>();    // Depend on TupleConverter
            container.AddInterfaces<Vector2IntConverter>(); // Depend on TupleConverter
            container.AddInterfaces<Vector3IntConverter>(); // Depend on TupleConverter
            container.AddInterfaces<ColorConverter>();      // Depend on TupleConverter
            container.AddInterfaces<Color32Converter>();    // Depend on TupleConverter

            #endregion

            #region Collections

            container.AddInterfaces<ArrayConverter>();
            container.AddInterfaces<CollectionConverter>();         // Depend on ArrayConverter
            container.AddInterfaces<AbstractCollectionConverter>(); // Depend on ArrayConverter
            container.AddInterfaces<DictionaryConverter>();         // Depend on ArrayConverter
            container.AddInterfaces<ReadOnlyDictionaryConverter>(); // Depend on DictionaryConverter
            container.AddInterfaces<AbstractDictionaryConverter>(); // Depend on DictionaryConverter

            #endregion

            #endregion

            container.AddInterfaces<ConverterManager>();
        }
    }
}
#endif