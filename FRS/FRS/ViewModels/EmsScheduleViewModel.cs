using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class EmsScheduleViewModel
    {
        public int Id { get; set; }


        public string Label { get; set; }
        public long? LocationId { get; set; }

        public string LocationCode { get; set; }

        public string LocationLabel { get; set; }

        public int? EmsProfileId { get; set; }

        public string EmsProfileCode { get; set; }

        public string EmsProfileLabel { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public DateTime? IneffectiveDate { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public bool ExceptHoliday { get; set; }

        public int? EmsGroupId { get; set; }
        public string EmsGroupName { get; set; }
    }
}
