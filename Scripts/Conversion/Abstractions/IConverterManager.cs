﻿#nullable enable
namespace TheOne.Data.Conversion
{
    using System;

    public interface IConverterManager
    {
        public IConverter GetConverter(Type type);

        public object? GetDefaultValue(Type type) => this.GetConverter(type).GetDefaultValue(type);

        public object ConvertFromString(string str, Type type) => this.GetConverter(type).ConvertFromString(str, type);

        public string ConvertToString(object obj, Type type) => this.GetConverter(type).ConvertToString(obj, type);
    }
}