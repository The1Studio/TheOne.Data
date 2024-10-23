#nullable enable
namespace TheOne.Data.Conversion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TheOne.Extensions;
    using UnityEngine.Scripting;

    public sealed class ConverterManager : IConverterManager
    {
        private readonly IReadOnlyList<IConverter> converters;

        private readonly Dictionary<Type, IConverter> converterCache = new Dictionary<Type, IConverter>();

        [Preserve]
        public ConverterManager(IEnumerable<IConverter> converters)
        {
            this.converters = converters.ToArray();
            this.converters.ForEach(converter => converter.Manager = this);
        }

        IConverter IConverterManager.GetConverter(Type type)
        {
            lock (this.converterCache)
            {
                return this.converterCache.GetOrAdd(type, () =>
                    this.converters.LastOrDefault(converter => converter.CanConvert(type))
                    ?? throw new ArgumentOutOfRangeException(nameof(type), type, $"No converter found for {type.Name}")
                );
            }
        }
    }
}