using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class TokenDishLabel : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int? token_label_id { get; set; }
        [ForeignKey("token_label_id")]
        public virtual TokenLabel TokenLabel { get; set; }
        public int? token_id { get; set; }
        [ForeignKey("token_id")]
        public virtual MealType Token { get; set; }
        public string token_name { get; set; }
        public int? dish_id { get; set; }
        public string dish_name { get; set; }
        public string dish_code { get; set; }
        public int? o_qty { get; set; }
        public int? p_qty { get; set; }
        public int? a_qty { get; set; }
        public int? t_qty { get; set; }
    }
}
