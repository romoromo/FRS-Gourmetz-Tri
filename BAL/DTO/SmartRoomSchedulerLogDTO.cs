using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class SmartRoomSchedulerLogDTO
    {
        public long Id { get; set; }
        public DateTime EventDateTime { get; set; }
        public string Status { get; set; }
        public int NoRecordsAffected { get; set; }
        public string Details { get; set; }
    }
}
