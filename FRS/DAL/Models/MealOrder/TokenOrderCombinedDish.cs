using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class TokenOrderCombinedDish : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? TokenOrderedId { get; set; }
        [ForeignKey("TokenOrderedId")]
        public virtual TokenOrdered TokenOrdered { get; set; }
        public string CDishLabel { get; set; }
        public string CDishCode { get; set; }
        public int? Qty { get; set; }
        public int? MenuQty { get; set; }
    }
}
