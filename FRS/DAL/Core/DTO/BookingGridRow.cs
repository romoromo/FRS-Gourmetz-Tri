using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{

    public class BookingGridLocation
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentLocationId { get; set; }
        public int Capacity { get; set; }
        public List<BookingGridLocationFacilitiy> Facilities { get; set; }
        public List<BookingGridLocation> Children { get; set; }
    }

    public class BookingGridLocationFacilitiy
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class BookingGridRow
    {
        public Location Location { get; set; }
        public List<LocationTimeInterval> LocationTimeIntervals { get; set; }
    }

    public class BookingGrid
    {
        public Location Location { get; set; }
        public List<BookingGridRow> Rows { get; set; }
    }

    public class LocationTimeInterval
    {
        public bool Selected { get; set; }
        public TimeInterval TimeInterval { get; set; }
        public Reservation Reservation { get; set; }

        public int Count { get; set; }
    }

    public class NextReservation
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string BookedBy { get; set; }
        public string TimeDisplay { get; set; }
        public int? InstitutionId { get; set; }
        public int ReservationId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }

    public class CurrentReservationDTO
    {
        public int ReservationId { get; set; }
        public int? InstitutionId { get; set; }
        public bool IsAvailable { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BookedBy { get; set; }
        public string TimeDisplay { get; set; }
        public string ReservationStatus { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCheckedIn { get; set; }
    }

    public class CurrentAvailableTimeSlot
    {
        public string Title { get; set; }
        public bool IsAvailable { get; set; }
        public string Status { get; set; }
        public string AvailableFrom { get; set; }
        public string AvailableTo { get; set; }
        public string AvailableTimeDisplay { get; set; }
        public int ReservationId { get; set; }
        public int? InstitutionId { get; set; }
        public string TimeDisplay { get; set; }
        public string ReservationStatus { get; set; }
        public string Description { get; set; }
        public string BookedBy { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class ExtendTime
    {
        public int ReservationId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string TimeDisplay { get; set; }
        public double TotalMinutes { get; set; }
    }

    public class LocationApiInformation
    {
        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
            set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public int event_id { get; set; }
        public string location_name { get; set; }
        public int location_capacity { get; set; }
        public string current_reservation_description { get; set; }
        public string current_reservation_status { get; set; }
        public string current_reservation_time_display { get; set; }
        public string current_reservation_organiser{ get; set; }
        public DateTime? next_reservation_time { get; set; }
        public string next_reservation_description { get; set; }
        public string next_reservation_time_display { get; set; }
        public string next_reservation_organiser { get; set; }

        public DateTime? current_reservation_start_time { get; set; }
        public DateTime? current_reservation_end_time { get; set; }
        public DateTime? next_reservation_start_time { get; set; }
        public DateTime? next_reservation_end_time { get; set; }

        public List<FacilitySimple> Facilities { get; set; }
    }

    public class FacilitySimple
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Path { get; set; }
        public string Filename { get; set; }
    }
}
