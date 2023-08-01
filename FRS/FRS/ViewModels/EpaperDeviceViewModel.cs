using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class EpaperDeviceViewModel
    {
        public int Id { get; set; }
        public string device_code { get; set; }
        public int device_status { get; set; }
        public string device_label { get; set; }

        public string mac_address { get; set; }
        public string ip_address { get; set; }
        public int host_address { get; set; }
        public DateTime? last_heartbeat { get; set; }

        public long? location_id { get; set; }
        public string location_code { get; set; }
        public string epaper_url { get; set; }
        public string status_display
        {
            get
            {
                return device_status == 0 ? "OFFLINE" : "ONLINE";
            }
        }
    }
}
