using DAL.Models.MealOrder;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EmailTemplate : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public int? InstitutionId { get; set; }
        public int? OutletId { get; set; }
        public virtual Outlet Outlet { get; set; }
        public virtual Institution Institution { get; set; }
    }
}
