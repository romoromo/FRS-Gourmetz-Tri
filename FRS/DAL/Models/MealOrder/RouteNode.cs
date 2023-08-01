using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class RouteNode: AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Route")]
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }

        [ForeignKey("Store")]
        public int StoreId { get; set; }
        public virtual StoreInfo Store { get; set; }

        public int order { get; set; }

        public DateTime interval { get; set; }
    }
}
