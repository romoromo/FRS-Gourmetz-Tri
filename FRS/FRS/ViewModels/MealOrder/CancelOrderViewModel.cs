using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels.MealOrder
{
    public class CancelOrderViewModel
    {
        public List<int> OrderIds { get; set; }
        public int CancelledBy { get; set; }
        public string Reason { get; set; }
    }
}
