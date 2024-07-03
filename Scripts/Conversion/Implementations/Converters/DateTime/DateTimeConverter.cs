#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class DateTimeConverter : Converter<DateTime>
    {
        private readonly string          format;
        private readonly IFormatProvider formatProvider;

        [Preserve]
        public DateTimeConverter(string format = "dd/MM/yyyy hh:mm:ss", IFormatProvider? formatProvider = null)
        {
            this.format         = format;
            this.formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return DateTime.ParseExact(str, this.format, this.formatProvider);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((DateTime)obj).ToString(this.format, this.formatProvider);
        }
    }
}