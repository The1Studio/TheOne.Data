#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEditor;
    using UnityEngine.Scripting;

    public sealed class UnityGuidConverter : Converter<GUID>
    {
        [Preserve]
        public UnityGuidConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return GUID.TryParse(str, out var guid) ? guid : throw new FormatException();
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}