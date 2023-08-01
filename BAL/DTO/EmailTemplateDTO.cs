using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class EmailTemplateDTO
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public int? InstitutionId { get; set; }
        public int? OutletId { get; set; }
    }
}
