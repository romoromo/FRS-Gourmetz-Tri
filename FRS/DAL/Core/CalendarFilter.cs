using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Core
{
    public class CalendarFilter
    {
        public int? capacity { get; set; }
        public List<int> locationIds { get; set; }
        public List<int> facilityIds { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? UpdatedFrom { get; set; }
        public DateTime? UpdatedTo { get; set; }
        public int? BookedById { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? HasAvailableTimeSlot { get; set; }
        public string Status { get; set; }
        public bool IsForAttendance { get; set; }
        public int? CurrentUserId { get; set; }
        public int? InstitutionId { get; set; }
        public bool IsAllEvents { get; set; }
        public string Email { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public bool IsForKiosk { get; set; }
    }
}
