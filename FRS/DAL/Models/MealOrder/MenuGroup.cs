using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MenuGroup : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime StartDate { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime EndDate { get; set; }

        public int? OutletId { get; set; }

        public bool IsPublished { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }

        public virtual ICollection<MenuGroupDishCycle> MenuGroupDishCycles { get; set; }
        public virtual ICollection<MenuGroupClass> Classes { get; set; }
    }
}
