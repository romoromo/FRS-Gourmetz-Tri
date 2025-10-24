using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DispenserOutlet : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string DispenserCode { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string CounterName { get; set; }

        public string Password { get; set; }

        public string LocationCode { get; set; }

        public string Color { get; set; }

        public string PLCIPAddress { get; set; }

        public string PLCPort { get; set; }

        public string PLCToken { get; set; }

        public int PLCApiVer { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public int? OutletId { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }

        public virtual ICollection<TrayModel> Trays { get; set; } = new HashSet<TrayModel>();
    }
}
