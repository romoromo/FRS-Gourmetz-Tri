using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class StudentGroup : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Code { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? OutletId { get; set; }

        public string Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublished { get; set; }

        public DateTime? DeliveryStartDate { get; set; }
        public DateTime? DeliveryEndDate { get; set; }
        public float Price { get; set; }
        public int Term { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? MealSessionId { get; set; }

        [ForeignKey("MealSessionId")]
        public virtual MealSession MealSession { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }
        public virtual ICollection<StudentGroupDetail> Sgdetails { get; set; }
    }
}
