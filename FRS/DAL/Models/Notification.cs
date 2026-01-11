using DAL.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Notification : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Header { get; set; }
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public bool IsRead { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("EventId")]
        public virtual NotificationEvent NotificationEvent { get; set; }

        public NotificationSettingType? Type { get; set; }
    }
}
