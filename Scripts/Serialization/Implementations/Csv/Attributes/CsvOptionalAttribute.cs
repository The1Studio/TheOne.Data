#if UNIT_CSV
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CsvOptionalAttribute : Attribute
    {
    }

    public static class CsvOptionalAttributeExtensions
    {
        public static bool IsCsvOptional(this FieldInfo field)
        {
            return field.GetCustomAttribute<CsvIgnoreAttribute>() is { }
                || field.ToPropertyInfo()?.GetCustomAttribute<CsvIgnoreAttribute>() is { };
        }
    }
}
#endif