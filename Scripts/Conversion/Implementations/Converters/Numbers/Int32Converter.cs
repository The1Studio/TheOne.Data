#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class Int32Converter : Converter<Int32>
    {
        private readonly IFormatProvider formatProvider;

        [Preserve]
        public Int32Converter(IFormatProvider? formatProvider = null)
        {
            this.formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Int32.Parse(str, this.formatProvider);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((Int32)obj).ToString(this.formatProvider);
        }
    }
}