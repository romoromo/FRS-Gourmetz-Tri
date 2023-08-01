using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Filters
{
    public class DevicePushMessageDTO
    {
        public List<int> DeviceIds { get; set; }
        public DashboardFilter Filter { get; set; }
        public string EmergencyMessage1 { get; set; }
        public string EmergencyMessage2 { get; set; }
        public string EmergencyMessage3 { get; set; }
        public string EmergencyMessage4 { get; set; }
    }
}
