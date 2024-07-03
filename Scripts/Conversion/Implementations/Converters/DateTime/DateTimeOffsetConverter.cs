#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class DateTimeOffsetConverter : Converter<DateTimeOffset>
    {
        private readonly string          format;
        private readonly IFormatProvider formatProvider;

        [Preserve]
        public DateTimeOffsetConverter(string format = "dd/MM/yyyy hh:mm:ss", IFormatProvider? formatProvider = null)
        {
            this.format         = format;
            this.formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return DateTimeOffset.ParseExact(str, this.format, this.formatProvider);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((DateTimeOffset)obj).ToString(this.format, this.formatProvider);
        }
    }
}