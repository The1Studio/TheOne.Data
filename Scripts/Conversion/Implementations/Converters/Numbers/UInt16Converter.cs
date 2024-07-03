#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class UInt16Converter : Converter<UInt16>
    {
        private readonly IFormatProvider formatProvider;

        [Preserve]
        public UInt16Converter(IFormatProvider? formatProvider = null)
        {
            this.formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return UInt16.Parse(str, this.formatProvider);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((UInt16)obj).ToString(this.formatProvider);
        }
    }
}