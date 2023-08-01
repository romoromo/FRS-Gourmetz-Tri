using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class CancelOrderRequest : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public int? StudentId { get; set; }

        [ForeignKey("OrderId")]
        public virtual TokenOrder TokenOrder { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
