using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class ReservationFeedbackViewModel
    {
        
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ReservationId { get; set; }
        public string Feedback { get; set; }
        public string Comment { get; set; }
    }
}
