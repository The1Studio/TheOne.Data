#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class Int16Converter : Converter<Int16>
    {
        private readonly IFormatProvider formatProvider;

        [Preserve]
        public Int16Converter(IFormatProvider? formatProvider = null)
        {
            this.formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Int16.Parse(str, this.formatProvider);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((Int16)obj).ToString(this.formatProvider);
        }
    }
}