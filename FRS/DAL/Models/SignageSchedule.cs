using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class SignageSchedule : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? PublicationId { get; set; }
        [ForeignKey("PublicationId")]
        public virtual SignagePublication Publication { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public DateTime? IneffectiveDate { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public virtual ICollection<SignageScheduleCompilation> Compilations { get; set; }
    }
}
