using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class SignagePublication : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
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

        public virtual ICollection<SignageSchedule> Schedules { get; set; }

        public virtual ICollection<SignagePublicationHistory> History { get; set; }
    }
}
