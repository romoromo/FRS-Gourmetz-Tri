using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core
{
    public class DeviceFilter
    {
        public bool? IsActive { get; set; }
        public bool? IsApproved { get; set; }
        public int? InstitutionId { get; set; }
    }
}
