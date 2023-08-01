using Microsoft.EntityFrameworkCore.ChangeTracking;
using DAL.Core.Audit.Auditors.Comparator;
using System;
using DAL.Models;

namespace DAL.Core.Audit.Auditors
{
    public class DeletetionLogDetailsAuditor : ChangeLogDetailsAuditor
    {
        public DeletetionLogDetailsAuditor(EntityEntry dbEntry, AuditLog log) : base(dbEntry, log) { }

        protected override bool IsValueChanged(string propertyName)
        {
            var propertyType = dbEntry.Entity.GetType()?.GetProperty(propertyName)?.PropertyType;
            object defaultValue = null;
            if (propertyType != null && propertyType.IsValueType)
                defaultValue = Activator.CreateInstance(propertyType);
            object orginalvalue = OriginalValue(propertyName);

            var comparator = ComparatorFactory.GetComparator(propertyType);

            return !comparator.AreEqual(defaultValue, orginalvalue);
        }

        protected override object CurrentValue(string propertyName)
        {
            return null;
        }
    }
}
