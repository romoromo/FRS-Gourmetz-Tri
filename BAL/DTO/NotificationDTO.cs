using DAL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public bool IsRead { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }

        public NotificationSettingType? Type { get; set; }
    }

    public class NotificationEventDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Template { get; set; }
    }

    public class NotificationSettingDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
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
