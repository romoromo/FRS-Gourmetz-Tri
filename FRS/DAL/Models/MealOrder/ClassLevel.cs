using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class ClassLevel : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int Year { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public int? OutletId { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }

        public int? MealSessionId { get; set; }

        [ForeignKey("MealSessionId")]
        public virtual MealSession MealSession { get; set; }

        public int? MealSessionDetailId { get; set; }

        [ForeignKey("MealSessionDetailId")]
        public virtual MealSessionDetail MealSessionDetail { get; set; }
    }
}
