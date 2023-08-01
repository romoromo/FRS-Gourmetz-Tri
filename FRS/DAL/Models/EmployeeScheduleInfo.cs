using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EmployeeScheduleInfo : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual EmployeeSchedule Schedule { get; set; }

        public int? LocationId { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        public int? ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public virtual EmployeeScheduleShift Shift { get; set; }

        public int? day { get; set; }

        public bool extend { get; set; }
        public bool noDisplay { get; set; }
    }
}
