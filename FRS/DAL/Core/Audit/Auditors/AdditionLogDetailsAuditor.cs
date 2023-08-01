using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DAL.Core.Audit.Auditors.Comparator;
using DAL.Core.Audit.Extensions;
using DAL.Models;

namespace DAL.Core.Audit.Auditors
{
    public class AdditionLogDetailsAuditor : ChangeLogDetailsAuditor
    {
        public AdditionLogDetailsAuditor(EntityEntry dbEntry, AuditLog log) : base(dbEntry, log) { }

        /// <summary>
        /// Treat unchanged entries as added entries when creating audit records.
        /// </summary>
        /// <returns></returns>
        protected internal override EntityState StateOfEntity()
        {
            if (dbEntry.State == EntityState.Unchanged)
                return EntityState.Added;

            return base.StateOfEntity();
        }

        protected override bool IsValueChanged(string propertyName)
        {
            var propertyType = dbEntry?.Entity?.GetType()?.GetProperty(propertyName)?.PropertyType;
            object defaultValue = propertyType?.DefaultValue();
            object currentValue = CurrentValue(propertyName);

            var comparator = ComparatorFactory.GetComparator(propertyType);

            return !comparator.AreEqual(defaultValue, currentValue);
        }

        protected override object OriginalValue(string propertyName)
        {
            return null;
        }
    }
}
