#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class UInt32Converter : Converter<UInt32>
    {
        private readonly IFormatProvider formatProvider;

        [Preserve]
        public UInt32Converter(IFormatProvider? formatProvider = null)
        {
            this.formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return UInt32.Parse(str, this.formatProvider);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((UInt32)obj).ToString(this.formatProvider);
        }
    }
}