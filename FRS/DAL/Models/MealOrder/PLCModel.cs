using Sieve.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.MealOrder
{
    public class PLCModel : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string IPAddress { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Framework { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int TotalNumber { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? OutletId { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }

        public virtual ICollection<TrayModel> Trays { get; set; } = new HashSet<TrayModel>();

    }
}
