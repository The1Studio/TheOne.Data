#nullable enable
namespace UniT.Data.Conversion
{
    using System;

    public interface IConverterManager
    {
        public IConverter GetConverter(Type type);

        public object? GetDefaultValue(Type type) => this.GetConverter(type).GetDefaultValue(type);

        public object ConvertFromString(string str, Type type) => this.GetConverter(type).ConvertFromString(str, type);

        public string ConvertToString(object obj, Type type) => this.GetConverter(type).ConvertToString(obj, type);

        public T? GetDefaultValue<T>() => (T?)this.GetDefaultValue(typeof(T));

        public T ConvertFromString<T>(string str) => (T)this.ConvertFromString(str, typeof(T));

        public string ConvertToString<T>(T obj) where T : notnull => this.ConvertToString(obj, typeof(T));
    }
}