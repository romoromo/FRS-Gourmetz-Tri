using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EmployeeSchedule : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? IneffectiveDate { get; set; }

        public virtual ICollection<EmployeeScheduleShift> Shifts { get; set; }
        public virtual ICollection<EmployeeScheduleLocation> Locations { get; set; }
        public virtual ICollection<EmployeeScheduleSlot> Slots { get; set; }
        public virtual ICollection<EmployeeScheduleInfo> Infos { get; set; }
    }
}
