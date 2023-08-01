using DAL.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DAL.Core.Audit.Auditors
{
    public class SoftDeletedLogDetailsAuditor : ChangeLogDetailsAuditor
    {
        public SoftDeletedLogDetailsAuditor(EntityEntry dbEntry, AuditLog log) : base(dbEntry, log) { }
    }
}
