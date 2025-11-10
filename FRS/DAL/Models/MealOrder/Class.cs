using DAL.Core;
using Sieve.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.MealOrder
{
    public class Class : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int ClassLevelId { get; set; }

        [ForeignKey("ClassLevelId")]
        public virtual ClassLevel ClassLevel { get; set; }

        public virtual ICollection<OutletClassRosterSchedulePeriodClass> Periods { get; set; }

        public MealCollectionType? MealCollectionType { get; set; }
    }
}
