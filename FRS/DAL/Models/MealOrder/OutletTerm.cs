using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OutletTerm : AuditableEntity
    {
        [Key]
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Label { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? OutletId { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }
    }
}
