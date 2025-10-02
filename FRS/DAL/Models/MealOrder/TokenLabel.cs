using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class TokenLabel : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int? meal_allocation_id { get; set; }
        [ForeignKey("meal_allocation_id")]
        public virtual MealAllocation Allocation { get; set; }
        public int? token_id { get; set; }
        [ForeignKey("token_id")]
        public virtual MealType Token { get; set; }
        public int? order_id { get; set; }
        public string token_name { get; set; }
        public int? qty { get; set; }
        public int? qty_dishes { get; set; }
        public int? qty_pdishes { get; set; }
        public int? qty_tdishes { get; set; }
        public int? qty_menus { get; set; }
        public string deliveryDate { get; set; }
        public virtual ICollection<TokenDishLabel> dishes { get; set; }
    }
}
