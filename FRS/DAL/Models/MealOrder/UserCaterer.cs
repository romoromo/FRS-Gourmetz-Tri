using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class UserCaterer : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CatererId { get; set; }

        [ForeignKey("CatererId")]
        public virtual CatererInfo Caterer { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
