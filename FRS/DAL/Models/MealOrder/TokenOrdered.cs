using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class TokenOrdered : AuditableEntity
    {
        public TokenOrdered()
        {
        }

        [Key]
        public int Id { get; set; }
        public int TokenId { get; set; }
        public string MealTypeId { get; set; }

        [ForeignKey("TokenId")]
        public virtual MealType Token { get; set; }
        public string TokenDesc { get; set; }
        public int Qty { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual TokenOrder Order { get; set; }

        public virtual ICollection<TokenOrderDish> SelectedDishes { get; set; }

        public virtual ICollection<TokenAltDish> TokenAltDishes { get; set; }

        public virtual ICollection<TokenOrderCombinedDish> SelectedCombinedDishes { get; set; }
    }
}
