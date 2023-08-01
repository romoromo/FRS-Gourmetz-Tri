using DAL.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DAL.Core.Audit.Auditors
{
    public class UnDeletedLogDetailsAuditor : ChangeLogDetailsAuditor
    {
        public UnDeletedLogDetailsAuditor(EntityEntry dbEntry, AuditLog log) : base(dbEntry, log) { }
    }
}
