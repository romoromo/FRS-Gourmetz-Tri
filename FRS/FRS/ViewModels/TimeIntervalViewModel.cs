using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class TimeIntervalViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }
        public string Value { get; set; }
        public int TotalMinutes { get; set; }

        public int ToTimeIntervalId { get; set; }
        public string ToTimeIntervalDescription { get; set; }
        public int ToTimeIntervalHour { get; set; }
        public int ToTimeIntervalMinutes { get; set; }
        public string ToTimeIntervalValue { get; set; }
    }
}
