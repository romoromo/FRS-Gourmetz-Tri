namespace DAL.Core.Audit.Auditors.Comparator
{
    internal class ValueTypeComparator : Comparator
    {
        internal override bool AreEqual(object value1, object value2)
        {
            return value1.Equals(value2);
        }
    }
}
