using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Reservation : AuditableEntity
    {
        public Reservation()
        {
            this.ReservationTimes = new HashSet<ReservationTime>();
            this.ReservationInvitees = new HashSet<ReservationInvitee>();
            this.ReservationContactGroups = new HashSet<ReservationContactGroup>();
        }
        [Key]
        public int Id { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string AlternateMeetingTitle { get; set; }

        public string DisplayTitle { get; set; }

        public string MeetingPurpose { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? RepeatEndDateTime { get; set; }
        public int? LocationId { get; set; }
        public bool IsAllDay { get; set; }
        public string Status { get; set; }

        public string StatusRemark { get; set; }

        public string RepeatType { get; set; }
        public bool IsReminder { get; set; }
        public int ReminderTotalMinutes { get; set; }

        public int? InstitutionId { get; set; }
        public DateTime? OriginalEndDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public string Notes { get; set; }
        public string OtherLocation { get; set; }
        public float? OtherLocationLat { get; set; }
        public float? OtherLocationLong { get; set; }
        public string CheckedInBy { get; set; }
        public bool IsAttendee { get; set; }
        public string OtherLocationUrl { get; set; }
        public string CancelReason { get; set; }
        public int? CancelledBy { get; set; }
        public DateTime? CancelledOn { get; set; }
        public string ReminderType { get; set; }
        public bool IsRecurring { get; set; }
        public int? ParentReservationId { get; set; }
        public string MeetingContactNo { get; set; }
        public bool IsKiosk { get; set; }
        public bool IsSignage { get; set; }
        public int? SmartRoomScheduleId { get; set; }

        public string DisplayTime { get; set; }

        [ForeignKey("SmartRoomScheduleId")]
        public virtual SmartRoomSchedule SmartRoomSchedule { get; set; }

        //public int? FileId { get; set; }

        //[ForeignKey("FileId")]
        //public virtual File kioskImage { get; set; }

        //[ForeignKey("ParentId")]
        [InverseProperty("ChildReservations")]
        public virtual Reservation ParentReservation { get; set; }
        public virtual ICollection<Reservation> ChildReservations { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<ReservationTime> ReservationTimes { get; set; }
        public virtual ICollection<ReservationInvitee> ReservationInvitees { get; set; }
        public virtual ICollection<ReservationContactGroup> ReservationContactGroups { get; set; }
        public virtual ICollection<VehicleEntry> VehicleEntries { get; set; }
        public virtual ICollection<ReservationPicture> ReservationPictures { get; set; }
    }
}
