using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EmsSchedule : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Label { get; set; }

        public long? LocationId { get; set; }
        public int? EmsProfileId { get; set; }

        public int? EmsGroupId { get; set; }

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

        public bool ExceptHoliday { get; set; }

        [ForeignKey("LocationId")]
        public virtual PIBTemplateLocation Location { get; set; }


        [ForeignKey("EmsProfileId")]
        public virtual EmsProfile EmsProfile { get; set; }

        [ForeignKey("EmsGroupId")]
        public virtual Ems EmsGroup { get; set; }
    }
}
