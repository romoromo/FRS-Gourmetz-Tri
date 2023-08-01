using System;

namespace DAL.Core.Audit.Auditors.Comparator
{
    internal static class ComparatorFactory
    {
        internal static Comparator GetComparator(Type type)
        {
            if (type == typeof(DateTime?))
            {
                return new NullableDateComparator();
            }

            if (type == typeof(DateTime))
            {
                return new DateComparator();
            }

            if (type == typeof(string))
            {
                return new StringComparator();
            }

            if (type != null && Nullable.GetUnderlyingType(type) != null)
            {
                return new NullableComparator();
            }

            if (type != null && type.IsValueType)
            {
                return new ValueTypeComparator();
            }

            return new Comparator();
        }
    }
}
