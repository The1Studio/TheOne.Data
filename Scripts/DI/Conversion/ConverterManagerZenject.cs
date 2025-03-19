﻿#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Conversion.DI
{
    using Zenject;

    public static class ConverterManagerZenject
    {
        public static void BindConverterManager(this DiContainer container)
        {
            if (container.HasBinding<IConverterManager>()) return;

            #region Converters

            #if UNIT_JSON
            container.BindInterfacesTo<JsonConverter>().AsSingle();
            #endif

            #region Numbers

            container.BindInterfacesTo<ByteConverter>().AsSingle();
            container.BindInterfacesTo<Int16Converter>().AsSingle();
            container.BindInterfacesTo<Int32Converter>().AsSingle();
            container.BindInterfacesTo<Int64Converter>().AsSingle();
            container.BindInterfacesTo<UInt16Converter>().AsSingle();
            container.BindInterfacesTo<UInt32Converter>().AsSingle();
            container.BindInterfacesTo<UInt64Converter>().AsSingle();
            container.BindInterfacesTo<SingleConverter>().AsSingle();
            container.BindInterfacesTo<DoubleConverter>().AsSingle();
            container.BindInterfacesTo<DecimalConverter>().AsSingle();

            #endregion

            #region DateTime

            container.BindInterfacesTo<DateTimeConverter>().AsSingle();
            container.BindInterfacesTo<DateTimeOffsetConverter>().AsSingle();

            #endregion

            #region Others

            container.BindInterfacesTo<BooleanConverter>().AsSingle();
            container.BindInterfacesTo<CharConverter>().AsSingle();
            container.BindInterfacesTo<StringConverter>().AsSingle();
            container.BindInterfacesTo<EnumConverter>().AsSingle();
            container.BindInterfacesTo<NullableConverter>().AsSingle();
            container.BindInterfacesTo<UriConverter>().AsSingle();
            container.BindInterfacesTo<GuidConverter>().AsSingle();
            container.BindInterfacesTo<UnityGuidConverter>().AsSingle();

            #endregion

            #region Tuples

            container.BindInterfacesTo<TupleConverter>().AsSingle();
            container.BindInterfacesTo<Vector2Converter>().AsSingle();    // Depend on TupleConverter
            container.BindInterfacesTo<Vector3Converter>().AsSingle();    // Depend on TupleConverter
            container.BindInterfacesTo<Vector4Converter>().AsSingle();    // Depend on TupleConverter
            container.BindInterfacesTo<Vector2IntConverter>().AsSingle(); // Depend on TupleConverter
            container.BindInterfacesTo<Vector3IntConverter>().AsSingle(); // Depend on TupleConverter
            container.BindInterfacesTo<ColorConverter>().AsSingle();      // Depend on TupleConverter
            container.BindInterfacesTo<Color32Converter>().AsSingle();    // Depend on TupleConverter

            #endregion

            #region Collections

            container.BindInterfacesTo<ArrayConverter>().AsSingle();
            container.BindInterfacesTo<CollectionConverter>().AsSingle();         // Depend on ArrayConverter
            container.BindInterfacesTo<AbstractCollectionConverter>().AsSingle(); // Depend on ArrayConverter
            container.BindInterfacesTo<DictionaryConverter>().AsSingle();         // Depend on ArrayConverter
            container.BindInterfacesTo<ReadOnlyDictionaryConverter>().AsSingle(); // Depend on DictionaryConverter
            container.BindInterfacesTo<AbstractDictionaryConverter>().AsSingle(); // Depend on DictionaryConverter

            #endregion

            #endregion

            container.BindInterfacesTo<ConverterManager>().AsSingle();
        }
    }
}
#endif