using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class Menu : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Label { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? InstitutionId { get; set; }

        public int? CuisineId { get; set; }

        [ForeignKey("CuisineId")]
        public virtual Cuisine Cuisine { get; set; }

        public int? CatererId { get; set; }

        [ForeignKey("CatererId")]
        public virtual CatererInfo Caterer { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public virtual ICollection<MenuDish> MenuDishes { get; set; }
    }
}
