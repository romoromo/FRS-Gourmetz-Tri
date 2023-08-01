using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MealType : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public double Price { get; set; }

        public string ChargeType { get; set; }

        public bool IsSubMenu { get; set; }

        public int? InstitutionId { get; set; }

        public int? CuisineId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        [ForeignKey("CuisineId")]
        public virtual Cuisine Cuisine { get; set; }

        [ForeignKey("Caterer")]
        public int? CatererId { get; set; }
        public virtual CatererInfo Caterer { get; set; }

        public virtual ICollection<MealTypeDish> Dishes { get; set; }
    }
}
