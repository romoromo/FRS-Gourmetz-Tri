using FRS.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class NotificationSettingViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        [Sanitize]
        public string Template { get; set; }
        public string AllowedEmails { get; set; }
        public string DayEnabled { get; set; }
        public bool IsEmailEnabled { get; set; }
        public bool IsAlertEnabled { get; set; }
        public int NumDaysBeforeCutoff { get; set; }
        public int NumHoursLeftCutoff { get; set; }
        public string Subject { get; set; }
    }
}
