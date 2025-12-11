using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models.MealOrder
{
    public class SortingArea : AuditableEntity
    {
        [Key]
        [Sieve(CanSort = true)]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Code { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? CatererInfoId { get; set; }
        public virtual CatererInfo CatererInfo { get; set; }

        public int? RouteId { get; set; }
        public virtual Route Route { get; set; }
    }
}
