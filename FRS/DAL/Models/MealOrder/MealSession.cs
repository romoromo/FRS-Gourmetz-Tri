using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MealSession : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int? InstitutionId { get; set; }

        public int Sequence { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? MealPeriodId { get; set; }

        public int? OutletId { get; set; }

        public int? CatererId { get; set; }

        [ForeignKey("CatererId")]
        public virtual CatererInfo Caterer { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }

        [ForeignKey("MealPeriodId")]
        public virtual MealPeriod MealPeriod { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public virtual ICollection<MealSessionDetail> Details { get; set; }

        public virtual ICollection<OutletClassRoster> ClassRosters { get; set; }
    }
}
