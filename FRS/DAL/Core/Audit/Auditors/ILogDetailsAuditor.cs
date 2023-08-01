using DAL.Models;
using System.Collections.Generic;

namespace DAL.Core.Audit.Auditors
{
    public interface ILogDetailsAuditor
    {
        IEnumerable<AuditLogDetail> CreateLogDetails();
    }
}
