#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class ByteConverter : Converter<Byte>
    {
        private readonly IFormatProvider formatProvider;

        [Preserve]
        public ByteConverter(IFormatProvider? formatProvider = null)
        {
            this.formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Byte.Parse(str, this.formatProvider);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((Byte)obj).ToString(this.formatProvider);
        }
    }
}