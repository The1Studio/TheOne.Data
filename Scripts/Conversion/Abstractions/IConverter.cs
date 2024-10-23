#nullable enable
namespace TheOne.Data.Conversion
{
    using System;

    public interface IConverter
    {
        public IConverterManager Manager { set; }

        public bool CanConvert(Type type);

        public object? GetDefaultValue(Type type);

        public object ConvertFromString(string str, Type type);

        public string ConvertToString(object obj, Type type);
    }
}