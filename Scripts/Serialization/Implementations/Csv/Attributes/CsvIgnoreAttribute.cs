#if THEONE_CSV
#nullable enable
namespace TheOne.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using TheOne.Extensions;

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CsvIgnoreAttribute : Attribute
    {
    }

    public static class CsvIgnoreAttributeExtensions
    {
        private static bool IsCsvIgnored(this FieldInfo field)
        {
            return field.GetCustomAttribute<CsvIgnoreAttribute>() is { };
        }

        public static (List<FieldInfo> NormalFields, List<FieldInfo> NestedFields) GetCsvFields(this Type type)
        {
            return type.GetAllFields()
                .Where(field => !field.IsCsvIgnored())
                .Split(field => !typeof(ICsvData).IsAssignableFrom(field.FieldType));
        }
    }
}
#endif