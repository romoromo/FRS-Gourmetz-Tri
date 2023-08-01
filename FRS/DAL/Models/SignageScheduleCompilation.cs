using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class SignageScheduleCompilation : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual SignageSchedule Schedule { get; set; }

        public int? CompilationId { get; set; }
        [ForeignKey("CompilationId")]
        public virtual SignageCompilation Compilation { get; set; }

        public int? interval { get; set; }
    }
}
