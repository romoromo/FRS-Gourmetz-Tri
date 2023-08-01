using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class LocationTimeSlot
    {
        public Location Location { get; set; }
        public List<Reservation> Reservations { get; set; }
        public string AvailableFrom { get; set; }
        public string AvailableTo { get; set; }
        public string AvailableTimeDisplay { get; set; }
    }
}
