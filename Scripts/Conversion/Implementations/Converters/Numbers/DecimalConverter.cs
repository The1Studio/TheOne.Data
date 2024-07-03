#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class DecimalConverter : Converter<Decimal>
    {
        private readonly IFormatProvider formatProvider;

        [Preserve]
        public DecimalConverter(IFormatProvider? formatProvider = null)
        {
            this.formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Decimal.Parse(str, this.formatProvider);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((Decimal)obj).ToString(this.formatProvider);
        }
    }
}