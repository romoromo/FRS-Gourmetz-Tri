using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EmailQueue : AuditableEntity
    {
        public EmailQueue()
        {
        }

        [Key]
        public int Id { get; set; }
        public string Action { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string ToName { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Cc { get; set; }
        public bool IsSent { get; set; }
        public bool IsFailed { get; set; }
        public string FailMessage { get; set; }
        public string Body { get; set; }
        public DateTime? SentDate { get; set; }
        public virtual Institution Institution { get; set; }
    }
}
