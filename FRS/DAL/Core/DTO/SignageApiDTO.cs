using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Core.DTO
{
    public class SignageLocationDTO
    {
        public int ID { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public int InstitutionID { get; set; }

        public string Category { get; set; }
    }

    public class SignageBookingDTO
    {
        public int ID { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public int? InstitutionID { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public int? LocationId { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public string Status { get; set; }

        public string BookingBy { get; set; }
    }
}
