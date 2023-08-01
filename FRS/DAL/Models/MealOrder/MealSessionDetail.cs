using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MealSessionDetail : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int? InstitutionId { get; set; }

        public int Sequence { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? RouteTime { get; set; }
        public DateTime? OverheadTime { get; set; }
        public DateTime? CalSourceTime { get; set; }

        public float RouteInterval { get; set; }
        public float OverheadInterval { get; set; }

        public int? RouteId { get; set; }

        public string MealSessionName { get; set; }

        public int MealSessionId { get; set; }

        [ForeignKey("MealSessionId")]
        public virtual MealSession MealSession { get; set; }

        [ForeignKey("RouteId")]
        public virtual Route Route { get; set; }
    }
}
