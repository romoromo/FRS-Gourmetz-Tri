using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class BookingGridRowViewModel
    {
        public LocationViewModel Location { get; set; }
        public List<LocationTimeIntervalViewModel> LocationTimeIntervals { get; set; }
    }

    public class BookingGridViewModel
    {
        public LocationViewModel Location { get; set; }
        public List<BookingGridRowViewModel> Rows { get; set; }
    }

    public class LocationTimeIntervalViewModel
    {
        public bool Selected { get; set; }
        public TimeIntervalViewModel TimeInterval { get; set; }
        public ReservationViewModel Reservation { get; set; }
        public int Count { get; set; }
    }
}
