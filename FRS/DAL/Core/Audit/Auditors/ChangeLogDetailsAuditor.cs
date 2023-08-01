using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DAL.Core.Audit.Auditors.Comparator;
using DAL.Core.Audit.Configuration;
using DAL.Core.Audit.Extensions;
using System.Collections.Generic;
using System.Linq;
using DAL.Models;

namespace DAL.Core.Audit.Auditors
{
    public class ChangeLogDetailsAuditor : ILogDetailsAuditor
    {
        protected readonly EntityEntry dbEntry;
        private readonly AuditLog log;

        public ChangeLogDetailsAuditor(EntityEntry dbEntry, AuditLog log)
        {
            this.dbEntry = dbEntry;
            this.log = log;
        }

        public IEnumerable<AuditLogDetail> CreateLogDetails()
        {
            var entityType = dbEntry.Entity.GetType()?.GetEntityType();
            foreach (string propertyName in PropertyNamesOfEntity())
            {
                if (PropertyTrackingConfiguration.IsTrackingEnabled(
                    new PropertyConfigurationKey(propertyName, entityType?.FullName), entityType)
                    && IsValueChanged(propertyName))
                {
                    yield return new AuditLogDetail
                    {
                        PropertyName = propertyName,
                        OriginalValue = OriginalValue(propertyName)?.ToString(),
                        NewValue = CurrentValue(propertyName)?.ToString(),
                        Log = log
                    };
                }
            }
        }

        protected internal virtual EntityState StateOfEntity()
        {
            return dbEntry.State;
        }

        private IEnumerable<string> PropertyNamesOfEntity()
        {
            return dbEntry.Metadata?.GetProperties().Select(s => s.Name);
        }

        protected virtual bool IsValueChanged(string propertyName)
        {
            var prop = dbEntry.Property(propertyName);
            var propertyType = dbEntry.Entity.GetType()?.GetProperty(propertyName)?.PropertyType;

            var originalValue = OriginalValue(propertyName);
            var comparator = ComparatorFactory.GetComparator(propertyType);

            var changed = (StateOfEntity() == EntityState.Modified
                && prop != null && prop.IsModified && !comparator.AreEqual(CurrentValue(propertyName), originalValue));
            return changed;
        }

        protected virtual object OriginalValue(string propertyName)
        {
            return dbEntry.Property(propertyName)?.OriginalValue;
        }

        protected virtual object CurrentValue(string propertyName)
        {
            return dbEntry.Property(propertyName)?.CurrentValue;
        }
    }
}
