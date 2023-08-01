using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core
{
    public class TimeIntervalFilter
    {
        public int? LocationId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeInterval StartTimeInterval { get; set; }
        public TimeInterval EndTimeInterval { get; set; }
        public bool IsIncludeReservations { get; set; }
    }
}
