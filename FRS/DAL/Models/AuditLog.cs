using DAL.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class AuditLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int? InstitutionId { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string InstitutionName { get; set; }

        public DateTime EventDateTime { get; set; }

        public AuditLogType LogType { get; set; }

        [Required]
        [MaxLength(256)]
        public string TableName { get; set; }

        [Required]
        [MaxLength(256)]
        public string RecordId { get; set; }
        public string GroupId { get; set; }
        public string ActionName { get; set; }
        public string Remarks { get; set; }

        public virtual ICollection<AuditLogDetail> Details { get; set; }
    }
}
