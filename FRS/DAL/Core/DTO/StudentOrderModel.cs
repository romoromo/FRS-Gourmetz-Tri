using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class StudentOrderModel
    {
        public Student Student { get; set; }
        public IEnumerable<TokenOrder> TokenOrders { get; set; }
    }
}
