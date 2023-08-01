using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DAL.Core.Audit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Models;

namespace DAL.Core.Audit.Auditors
{
    internal class LogAuditor : IDisposable
    {
        private readonly EntityEntry dbEntry;
        private readonly DbContext context;

        internal LogAuditor(EntityEntry dbEntry, DbContext context) { this.dbEntry = dbEntry; this.context = context; }

        public void Dispose() { }

        internal AuditLog CreateLogRecord(int? institutionId, int? userId, string userName, string institutionName, AuditLogType eventType)
        {
            Type entityType = GetEntityType(dbEntry.Entity.GetType());
            if (!EntityTrackingConfiguration.IsTrackingEnabled(entityType))
                return null;

            DateTime changeTime = DateTime.Now;
            var newlog = new AuditLog
            {
                InstitutionId = institutionId,
                UserId = userId,
                UserName = userName,
                InstitutionName = institutionName,
                EventDateTime = changeTime,
                LogType = eventType,
                TableName = entityType.FullName,
                RecordId = this.GetPrimaryKeyValueOf(dbEntry)
            };
            var detailsAuditor = GetDetailsAuditor(eventType, newlog);
            newlog.Details = detailsAuditor.CreateLogDetails().ToList();

            if (newlog.Details.Any())
                return newlog;
            else
                return null;
        }

        private ChangeLogDetailsAuditor GetDetailsAuditor(AuditLogType eventType, AuditLog newlog)
        {
            switch (eventType)
            {
                case AuditLogType.Added:
                    return new AdditionLogDetailsAuditor(dbEntry, newlog);

                case AuditLogType.Deleted:
                    return new DeletetionLogDetailsAuditor(dbEntry, newlog);

                case AuditLogType.Modified:
                    return new ChangeLogDetailsAuditor(dbEntry, newlog);

                case AuditLogType.SoftDeleted:
                    return new SoftDeletedLogDetailsAuditor(dbEntry, newlog);

                case AuditLogType.UnDeleted:
                    return new UnDeletedLogDetailsAuditor(dbEntry, newlog);

                default:
                    return null;
            }
        }

        private string GetPrimaryKeyValueOf(EntityEntry entry)
        {
            var keys = entry.Metadata.FindPrimaryKey().Properties;
            if (keys.Count == 1)
                return entry.Property(keys[0].Name).CurrentValue?.ToString();
            if (keys.Count > 1)
            {
                var tmp = new List<string>();
                foreach (var key in keys)
                {
                    tmp.Add(entry.Property(key.Name).CurrentValue?.ToString());
                }
                return "[" + string.Join(",", tmp) + "]";
            }
            throw new KeyNotFoundException("key not found for " + entry.Entity.GetType().FullName);
        }

        private Type GetEntityType(Type entityType)
        {
            var proxyNamespace = @"System.Data.Entity.DynamicProxies";
            if (entityType.Namespace == proxyNamespace)
                return GetEntityType(entityType.BaseType);

            proxyNamespace = @"Castle.Proxies";
            if (entityType.Namespace == proxyNamespace)
                return GetEntityType(entityType.BaseType);

            return entityType;
        }
    }
}
