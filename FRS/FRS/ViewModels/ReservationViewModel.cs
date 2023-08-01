using FRS.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class ReservationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brief description is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Brief description must be between 2 and 200 characters")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Brief description is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Brief description must be between 2 and 200 characters")]
        public string LongDescription { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm}")]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        public string AlternateMeetingTitle { get; set; }

        public string DisplayTitle { get; set; }

        public string MeetingPurpose { get; set; }

        public DateTime? OriginalEndDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }

        public DateTime? RepeatEndDateTime { get; set; }

        public int InstitutionId { get; set; }

        public int? LocationId { get; set; }

        public bool IsAttendee { get; set; }

        public bool IsAllDay { get; set; }


        public string DisplayTime { get; set; }

        public string IsAllDayYesNo
        {
            get
            {
                return IsAllDay ? "Yes" : "No";
            }
        }

        public string LocationName { get; set; }

        public string Status { get; set; }

        public string StatusRemark { get; set; }

        public string RepeatType { get; set; }

        public bool IsReminder { get; set; }

        public int ReminderTotalMinutes { get; set; }

        public string BookedBy { get; set; }

        public string BookedEmail { get; set; }

        public string BookedUserName { get; set; }

        public string Notes { get; set; }

        public string MeetingContactNo { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public string OtherLocation { get; set; }
        public float? OtherLocationLat { get; set; }
        public float? OtherLocationLong { get; set; }
        public string OtherLocationUrl { get; set; }
        public string ReminderType { get; set; }

        public string CreatedByName { get; set; }
        public string LastUpdatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public int? ParentReservationId { get; set; }
        public int? SmartRoomScheduleId { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurApplyChangesType { get; set; }
        public string CheckedInBy { get; set; }
        public string RequestedBy { get; set; }
        public bool IsKiosk { get; set; }
        public bool IsSignage { get; set; }
        //[JsonIgnore]
        public TimeIntervalViewModel StartTime
        {
            get
            {
                return (this.TimeIntervals != null && this.TimeIntervals.Count > 0 && !this.TimeIntervals.Any(e => e == null)) ? this.TimeIntervals.OrderBy(e => e.Hour).ThenBy(e => e.Minutes).First() : null;
            }
        }

        //[JsonIgnore]
        public TimeIntervalViewModel EndTime
        {
            get
            {
                return (this.TimeIntervals != null && this.TimeIntervals.Count > 0 && !this.TimeIntervals.Any(e => e == null)) ?
                    this.TimeIntervals.OrderBy(e => e.Hour).ThenBy(e => e.Minutes).Last() : null;
            }
        }

        //[JsonIgnore]
        public List<FacilityViewModel> Facilities { get; set; }
        public List<FacilityTypeViewModel> FacilityTypes { get; set; }
        //[JsonIgnore]
        public List<TimeIntervalViewModel> TimeIntervals { get; set; }
        public List<ReservationInviteeViewModel> Invitees { get; set; }
        //[JsonIgnore]
        public List<ContactGroupViewModel> ContactGroups { get; set; }

        public LocationViewModel LocationObj { get; set; }

        public string Link { get; set; }

        public string FilePath { get; set; }

        public byte[] FileBytes { get; set; }

        public List<ReservationPictureViewModel> ReservationPictures { get; set; }

    }
}
