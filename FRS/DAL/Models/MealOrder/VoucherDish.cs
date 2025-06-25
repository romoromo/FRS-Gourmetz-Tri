using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.MealOrder
{
    public class VoucherDish: AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Voucher")]
        public int VoucherId { get; set; }
        public virtual Voucher Voucher { get; set; }

        [ForeignKey("Dish")]
        public int DishId { get; set; }
        public virtual Dish Dish { get; set; }
    }
}
