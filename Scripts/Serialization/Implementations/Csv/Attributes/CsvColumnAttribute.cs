#if THEONE_CSV
#nullable enable
namespace TheOne.Data.Serialization
{
    using System;
    using System.Reflection;
    using TheOne.Extensions;

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CsvColumnAttribute : Attribute
    {
        public string Name         { get; }
        public bool   IgnorePrefix { get; }

        public CsvColumnAttribute(string name, bool ignorePrefix = false)
        {
            this.Name         = name;
            this.IgnorePrefix = ignorePrefix;
        }
    }

    public static class CsvColumnAttributeExtensions
    {
        public static string GetCsvColumn(this FieldInfo field, string prefix)
        {
            return field.GetCustomAttribute<CsvColumnAttribute>() is { } attr
                ? attr.IgnorePrefix ? attr.Name : prefix + attr.Name
                : prefix + field.Name.ToPropertyName();
        }
    }
}
#endif