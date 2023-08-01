using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class AuditLogDetailDTO
    {
        public long Id { get; set; }

        public string PropertyName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public long AuditLogId { get; set; }
    }
}
