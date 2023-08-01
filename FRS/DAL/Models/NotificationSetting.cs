using DAL.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class NotificationSetting : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public NotificationSettingType Type { get; set; }
        public string Template { get; set; }
        public string AllowedEmails { get; set; }
        public DayOfWeek DayEnabled { get; set; }
        public bool IsEmailEnabled { get; set; }
        public bool IsAlertEnabled { get; set; }
        public int NumDaysBeforeCutoff { get; set; }
        public int NumHoursLeftCutoff { get; set; }
        public string Subject { get; set; }
    }
}
