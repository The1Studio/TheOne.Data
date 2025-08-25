#if THEONE_CSV
#nullable enable
namespace TheOne.Data
{
    using System;
    using System.Collections;

    public interface ICsvData
    {
        public Type RowType { get; }

        public void Add(object key, object value);

        public IEnumerator GetValues();
    }
}
#endif