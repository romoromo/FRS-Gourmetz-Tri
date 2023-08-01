using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
    }
}
