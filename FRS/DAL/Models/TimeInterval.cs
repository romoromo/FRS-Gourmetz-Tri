using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class TimeInterval : AuditableEntity
    {
        public TimeInterval()
        {

        }

        public TimeInterval(string description, string value, int hour, int minutes)
        {
            this.Description = description;
            this.Value = value;
            this.Hour = hour;
            this.Minutes = minutes;
        }

        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }
        public string Value { get; set; }

        public int? ToTimeIntervalId { get; set; }
        public virtual TimeInterval ToTimeInterval { get; set; }
    }
}
