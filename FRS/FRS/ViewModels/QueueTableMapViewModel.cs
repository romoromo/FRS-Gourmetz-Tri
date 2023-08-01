using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class QueueTableMapViewModel
    {
        public int Id { get; set; }
        public string QueueId { get; set; }
        public string DeviceId { get; set; }
        public string StationId { get; set; }

        public string RoomName { get; set; }

        public string CurrentDisplay { get; set; }

        public string LastCall { get; set; }

        public string LastReturn { get; set; }

        public bool IsActive { get; set; }

        public int? DeviceIdentifier { get; set; }
    }
}
