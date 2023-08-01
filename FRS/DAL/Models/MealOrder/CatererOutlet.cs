using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class CatererOutlet : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CatererInfo")]
        public int CatererInfoId { get; set; }
        [ForeignKey("Outlet")]
        public int OutletId { get; set; }

        [ForeignKey("OutletProfile")]
        public int? OutletProfileId { get; set; }

        public bool IsRsp { get; set; }
        public string Status { get; set; }
        public virtual Outlet Outlet { get; set; }
        public virtual CatererInfo CatererInfo { get; set; }
        public virtual OutletProfile OutletProfile { get; set; }
    }
}
