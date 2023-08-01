using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EmployeeScheduleSlot : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual EmployeeSchedule Schedule { get; set; }

        public int? EmployeeDataId { get; set; }
        [ForeignKey("EmployeeDataId")]
        public virtual EmployeeData EmployeeData { get; set; }


        public int? CoveringEmployeeDataId { get; set; }
        [ForeignKey("CoveringEmployeeDataId")]
        public virtual EmployeeData CoveringEmployeeData { get; set; }

        public int? LocationId { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        public int? ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public virtual EmployeeScheduleShift Shift { get; set; }

        public int? day { get; set; }
    }
}
