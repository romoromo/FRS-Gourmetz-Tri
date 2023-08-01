
using System;
using System.Collections.Generic;

namespace FRS.ViewModels
{
    public class SignageDashboardViewModel
    {
        public int Unreachable { get; set; }
        public int Asleep { get; set; }
        public int Alive { get; set; }

        public List<DeviceViewModel> Devices { get; set; }
        public PagedEntityViewModel<DeviceViewModel> PagedDevices { get; set; }
    }
}
