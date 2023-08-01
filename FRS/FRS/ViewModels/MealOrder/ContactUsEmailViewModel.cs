using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels.MealOrder
{
    public class ContactUsEmailViewModel
    {
        public int UserId { get; set; }
        public int StudentId { get; set; }
        public string Subject { get; set; }
        public string Detail { get; set; }
    }
}
