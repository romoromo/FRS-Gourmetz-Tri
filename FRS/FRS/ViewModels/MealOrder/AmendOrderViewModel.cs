using BAL.DTO.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels.MealOrder
{
    public class AmendOrderViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string InvoiceNumber { get; set; }
        public string FomoId { get; set; }
        public int? UpdatedById { get; set; }
        public string Reason { get; set; }
    }
}
