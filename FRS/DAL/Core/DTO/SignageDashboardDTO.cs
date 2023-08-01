using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class SignageDashboardDTO
    {
        public int Unreachable { get; set; }
        public int Asleep { get; set; }
        public int Alive { get; set; }

        public List<Device> Devices { get; set; }
        public PagedEntity<Device> PagedDevices { get; set; }
    }
}
