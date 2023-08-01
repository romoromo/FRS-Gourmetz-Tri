using System;

namespace FRS.ViewModels
{
    public class OccupancyLogViewModel
    {
        public int Id { get; set; }
        public DateTime? Datetime { get; set; }
        public string DeviceId { get; set; }
        public string SensorId { get; set; }
        public string Status { get; set; }
    }
}
