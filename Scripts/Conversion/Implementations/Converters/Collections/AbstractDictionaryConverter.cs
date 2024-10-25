#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    /// <summary>
    ///     Depends on <see cref="DictionaryConverter"/>
    /// </summary>
    public sealed class AbstractDictionaryConverter : Converter
    {
        private static readonly Type[] SupportedTypes =
        {
            typeof(IDictionary<,>),
            typeof(IReadOnlyDictionary<,>),
        };

        [Preserve]
        public AbstractDictionaryConverter()
        {
        }

        protected override bool CanConvert(Type type) => SupportedTypes.Any(type.IsGenericTypeOf);

        protected override object? GetDefaultValue(Type type)
        {
            return this.Manager.GetDefaultValue(MakeDictionaryType(type));
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return this.Manager.ConvertFromString(str, MakeDictionaryType(type));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return this.Manager.ConvertToString(obj, MakeDictionaryType(type));
        }

        private static Type MakeDictionaryType(Type type)
        {
            return typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
        }
    }
}