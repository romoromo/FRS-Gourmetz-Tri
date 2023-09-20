using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core
{
    public class AuditUserActivityType
    {
        public string GroupId { get; set; }
        public string ActionName { get; set; }
        public string Remarks { get; set; }
    }
}
