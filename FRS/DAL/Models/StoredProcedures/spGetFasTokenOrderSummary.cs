using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace DAL.Models.StoredProcedures
{
    [NotMapped]
    public class spGetFasTokenOrderSummary
    {
        [Key]
        public long Id { get; set; }
        public int SessionId { get; set; }
        public string Session { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DishTypeName { get; set; }
        public int Quantity { get; set; }
    }
}
