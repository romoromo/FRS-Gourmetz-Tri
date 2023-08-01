using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class SmartRoomSchedule
    {
        public SmartRoomSchedule()
        {
        }

        [Key]
        public int Id { get; set; }

        public int SmartRoomScheduleID { get; set; }
        public string AlternateID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int? RoomID { get; set; }
        public string AlternateRoomID { get; set; }
        public int? UserID { get; set; }
        public string UserName { get; set; }
        public string MeetingContactNo { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ReserveSource { get; set; }
        public string Status { get; set; }
        public string StatusRemarks { get; set; }
        public string Remarks { get; set; }
        public string RequestorID { get; set; }
        public string Requestor { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? StartedBy { get; set; }
        public DateTime? StartedAt { get; set; }
        public int? EndedBy { get; set; }
        public DateTime? EndedAt { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }
    }
}
