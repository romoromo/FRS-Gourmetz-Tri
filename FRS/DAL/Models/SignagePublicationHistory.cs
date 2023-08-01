using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class SignagePublicationHistory : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public bool Approved { get; set; }
        public bool Rejected { get; set; }
        public string Remark { get; set; }

        public int? ApprovedBy { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual ApplicationUser ApprovedByUser { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public int? RejectedBy { get; set; }

        [ForeignKey("RejectedBy")]
        public virtual ApplicationUser RejectedByUser { get; set; }

        public DateTime? RejectedDate { get; set; }

        public int? PublicationId { get; set; }
        [ForeignKey("PublicationId")]
        public virtual SignagePublication Publication { get; set; }
    }
}
